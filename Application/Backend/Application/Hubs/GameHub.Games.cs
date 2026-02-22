using Backend.Application.DTOs.Games;
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
}
