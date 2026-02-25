namespace Backend.Domain.Commands.Handlers;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Commands.Draw;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class AdvancePhaseActionCommandHandler : IGameCommandHandler<AdvancePhaseActionCommand, PhaseAdvanceResult>
{
    public async Task<PhaseAdvanceResult> HandleAsync(AdvancePhaseActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var turnChanged = false;
        Guid? activePlayerId = context.CurrentTurn.ActivePlayerId;
        var turnForResult = context.CurrentTurn;

        switch (context.CurrentTurn.Phase)
        {
            case TurnPhase.Draw:
                if (context.Actor.TurnEnded)
                    throw new BadRequestException("You already clicked Next in this phase.");

                MarkActorAsEnded(context);
                await DrawPhaseCoordinator.TryAdvanceToMain1Async(context);
                activePlayerId = context.CurrentTurn.ActivePlayerId;
                break;

            case TurnPhase.Main1:
                if (context.Actor.TurnEnded)
                    throw new BadRequestException("You already clicked Next in this phase.");

                activePlayerId = await AdvanceMain1PhaseAsync(context);
                break;

            case TurnPhase.Battle:
                if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
                    throw new BadRequestException("Only active player can advance Battle phase.");

                (turnForResult, activePlayerId, turnChanged) = await AdvanceBattlePhaseAsync(context);
                break;

            default:
                throw new BadRequestException("Unsupported phase transition.");
        }

        return new PhaseAdvanceResult(
            Game: context.Game,
            Turn: turnForResult,
            Player: context.Actor,
            ActivePlayerId: activePlayerId,
            CurrentPhase: turnForResult.Phase,
            TurnChanged: turnChanged
        );
    }

    private static void MarkActorAsEnded(GameCommandContext context)
    {
        if (context.Actor.TurnEnded)
            return;

        context.Actor.TurnEnded = true;
        context.UnitOfWork.PlayerGames.Update(context.Actor);
    }

    private static async Task<Guid?> AdvanceMain1PhaseAsync(GameCommandContext context)
    {
        MarkActorAsEnded(context);

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        var everyoneEnded = players.All(player => player.TurnEnded);
        if (!everyoneEnded)
        {
            if (context.CurrentTurn.ActivePlayerId != null)
            {
                context.CurrentTurn.ActivePlayerId = null;
                context.UnitOfWork.Turns.Update(context.CurrentTurn);
            }

            return null;
        }

        foreach (var player in players)
        {
            player.TurnEnded = false;
            context.UnitOfWork.PlayerGames.Update(player);
        }

        var battleStarter = players[(context.CurrentTurn.TurnNumber - 1) % players.Count];
        context.CurrentTurn.Phase = TurnPhase.Battle;
        context.CurrentTurn.ActivePlayerId = battleStarter.Id;
        context.CurrentTurn.StartedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.CurrentTurn);

        return battleStarter.Id;
    }

    private static async Task<(Turn Turn, Guid? ActivePlayerId, bool TurnChanged)> AdvanceBattlePhaseAsync(GameCommandContext context)
    {
        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        MarkActorAsEnded(context);

        var nextBattlePlayer = GetNextNotEndedPlayer(players, context.Actor.Id);
        if (nextBattlePlayer != null)
        {
            context.CurrentTurn.ActivePlayerId = nextBattlePlayer.Id;
            context.UnitOfWork.Turns.Update(context.CurrentTurn);
            return (context.CurrentTurn, nextBattlePlayer.Id, false);
        }

        context.CurrentTurn.EndedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.CurrentTurn);

        var nextTurn = await context.UnitOfWork.Turns.NextTurnAsync(context.CurrentTurn);
        nextTurn.ActivePlayerId = null;
        nextTurn.Phase = TurnPhase.Draw;

        foreach (var player in players)
        {
            player.TurnEnded = false;
            context.UnitOfWork.PlayerGames.Update(player);
        }

        return (nextTurn, null, true);
    }

    private static PlayerGame? GetNextNotEndedPlayer(List<PlayerGame> players, Guid currentPlayerId)
    {
        var currentIndex = players.FindIndex(player => player.Id == currentPlayerId);
        if (currentIndex < 0)
            throw new BadRequestException("Current battle player not found in game players.");

        for (var step = 1; step < players.Count; step++)
        {
            var candidate = players[(currentIndex + step) % players.Count];
            if (!candidate.TurnEnded)
                return candidate;
        }

        return null;
    }
}
