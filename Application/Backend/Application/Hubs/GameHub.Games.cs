using Backend.Application.DTOs.Games;
using Backend.Domain;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.SignalR;

public partial class GameHub
{
    [HubMethodName("game:join")]
    public async Task JoinGame(Guid gameId)
    {
        var userId = GetUserId();
        if (!await Games.UserExistsInGame(gameId, userId))
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, GetGameGroupName(gameId));
    }

    [HubMethodName("game:leave")]
    public async Task LeaveGame(Guid gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGameGroupName(gameId));
    }

    [HubMethodName("game:action:draw")]
    public async Task DrawCardAction(Guid gameId)
    {
        try
        {
            var userId = GetUserId();
            var result = await Games.DrawCard(gameId, userId);

            await Clients.Caller.SendAsync("game:draw:result", result);

            var publicEvent = new PlayerDrewCardEventDto
            {
                GameId = result.GameId,
                PlayerGameId = result.PlayerGameId,
                TurnId = result.TurnId,
                DrawsInTurn = result.DrawsInTurn,
                TurnEnded = result.TurnEnded,
                PhaseAdvanced = result.PhaseAdvanced,
                CurrentPhase = result.CurrentPhase,
            };

            await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
                .SendAsync("game:player:drew", publicEvent);
        }
        catch (BadRequestException exception)
        {
            throw new HubException(exception.Message);
        }
        catch (ObjectNotFoundException exception)
        {
            throw new HubException(exception.Message);
        }
    }

    [HubMethodName("game:action:draw:skip")]
    public async Task SkipDrawAction(Guid gameId)
    {
        var userId = GetUserId();
        var result = await Games.SkipDraw(gameId, userId);

        await Clients.Caller.SendAsync("game:draw:skip:result", result);

        var publicEvent = new PlayerSkippedDrawEventDto
        {
            GameId = result.GameId,
            PlayerGameId = result.PlayerGameId,
            TurnId = result.TurnId,
            TurnEnded = result.TurnEnded,
            PhaseAdvanced = result.PhaseAdvanced,
            CurrentPhase = result.CurrentPhase,
        };

        await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
            .SendAsync("game:player:draw:skipped", publicEvent);
    }

    [HubMethodName("game:action:place")]
    public async Task PlaceCardAction(Guid gameId, Guid gameCardId, int fieldIndex, bool faceDown)
    {
        var userId = GetUserId();
        var result = await Games.PlaceCard(gameId, userId, gameCardId, fieldIndex, faceDown);

        await Clients.Caller.SendAsync("game:place:result", result);

        await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
            .SendAsync("game:player:placed", new
            {
                result.GameId,
                result.PlayerGameId,
                result.TurnId,
                result.GameCardId,
                result.FieldIndex,
                result.FaceDown,
                result.CurrentPhase,
            });

        if (result.ActivatedEffect is not null)
        {
            await Clients.Group(GetGameGroupName(gameId))
                .SendAsync("game:effect:activated", result.ActivatedEffect);
        }
    }

    [HubMethodName("game:action:grave")]
    public async Task SendCardToGraveyardAction(Guid gameId, Guid gameCardId)
    {
        var userId = GetUserId();
        var result = await Games.SendCardToGraveyard(gameId, userId, gameCardId);

        await Clients.Caller.SendAsync("game:grave:result", result);

        await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
            .SendAsync("game:player:card:updated", result);
    }

    [HubMethodName("game:action:toggle-defense")]
    public async Task ToggleDefensePositionAction(Guid gameId, Guid gameCardId)
    {
        var userId = GetUserId();
        var result = await Games.ToggleDefensePosition(gameId, userId, gameCardId);

        await Clients.Caller.SendAsync("game:toggle:defense:result", result);

        await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
            .SendAsync("game:player:card:updated", result);
    }

    [HubMethodName("game:action:reveal")]
    public async Task RevealCardAction(Guid gameId, Guid gameCardId)
    {
        var userId = GetUserId();
        var result = await Games.RevealCard(gameId, userId, gameCardId);

        await Clients.Caller.SendAsync("game:reveal:result", result);

        await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
            .SendAsync("game:player:card:updated", result);
    }

    [HubMethodName("game:action:attack")]
    public async Task<BattleAttackResultDto> AttackAction(
        Guid gameId,
        Guid attackerCardId,
        Guid? defenderCardId,
        Guid? defenderPlayerGameId)
    {
        var userId = GetUserId();
        Guid? activatedTrapCardId = null;

        var trapWindow = await Games.GetTrapResponseWindow(
            gameId,
            userId,
            attackerCardId,
            defenderCardId,
            defenderPlayerGameId);

        if (trapWindow is not null)
        {
            var trapCardIds = trapWindow.AvailableTrapCards.Select(card => card.GameCardId).ToList();
            if (TrapResponses.TryOpenWindow(
                gameId,
                trapWindow.DefenderUserId,
                trapWindow.DefenderPlayerGameId,
                trapCardIds,
                out var windowId,
                out var expiresAtUtc))
            {
                await Clients.User(trapWindow.DefenderUserId.ToString())
                    .SendAsync("game:effect:trap:window", new
                    {
                        gameId,
                        windowId,
                        defenderPlayerGameId = trapWindow.DefenderPlayerGameId,
                        timeoutSeconds = GameConstants.TrapResponseWindowSeconds,
                        expiresAtUtc,
                        availableTrapCards = trapWindow.AvailableTrapCards,
                    });

                await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
                    .SendAsync("game:effect:trap:window:opened", new
                    {
                        gameId,
                        defenderPlayerGameId = trapWindow.DefenderPlayerGameId,
                        timeoutSeconds = GameConstants.TrapResponseWindowSeconds,
                    });

                activatedTrapCardId = await TrapResponses.WaitForSelectionOrTimeoutAsync(windowId);
            }
        }

        var result = await Games.Attack(gameId, userId, attackerCardId, defenderCardId, defenderPlayerGameId, activatedTrapCardId);

        await Clients.Caller.SendAsync("game:attack:result", result);

        await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
            .SendAsync("game:player:attacked", result);

        if (result.ActivatedEffect is not null)
        {
            await Clients.Group(GetGameGroupName(gameId))
                .SendAsync("game:effect:activated", result.ActivatedEffect);
        }

        return result;
    }

    [HubMethodName("game:action:trap:activate")]
    public Task ActivateTrapResponse(Guid gameId, Guid windowId, Guid trapGameCardId)
    {
        var userId = GetUserId();
        var success = TrapResponses.TryActivateTrap(windowId, userId, trapGameCardId);
        if (!success)
            throw new HubException("Trap response window expired or trap is not valid.");

        return Clients.User(userId.ToString())
            .SendAsync("game:effect:trap:accepted", new
            {
                gameId,
                windowId,
                trapGameCardId,
            });
    }

    [HubMethodName("game:action:next")]
    public async Task AdvancePhaseAction(Guid gameId)
    {
        var userId = GetUserId();
        var result = await Games.AdvancePhase(gameId, userId);

        await Clients.Caller.SendAsync("game:next:result", result);

        await Clients.GroupExcept(GetGameGroupName(gameId), [Context.ConnectionId])
            .SendAsync("game:phase:advanced", result);
    }
}
