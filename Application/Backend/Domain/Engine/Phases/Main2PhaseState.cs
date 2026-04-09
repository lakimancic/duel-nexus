namespace Backend.Domain.Engine.Phases;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Utils.WebApi;

public sealed class Main2PhaseState : ITurnPhaseState
{
    private static readonly TimeSpan Main2PhaseTimeout = TimeSpan.FromMinutes(1);

    public TurnPhase Phase => TurnPhase.Main2;
    public TimeSpan? Timeout => Main2PhaseTimeout;

    public async Task<TurnPhaseTransitionResult> AdvanceAsync(
        TurnPhaseStateContext context,
        TurnPhaseAdvanceTrigger trigger,
        CancellationToken cancellationToken = default)
    {
        switch (trigger)
        {
            case TurnPhaseAdvanceTrigger.PlayerAdvance:
                MarkActorAsEndedIfNeeded(context.Actor, context);
                break;

            case TurnPhaseAdvanceTrigger.Timeout:
                break;

            default:
                throw new BadRequestException("Unsupported trigger for Main2 phase.");
        }

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        var alivePlayers = players.Where(player => player.LifePoints > 0).ToList();
        if (alivePlayers.Count == 0)
            throw new BadRequestException("Game has no active players.");

        var everyoneEnded = alivePlayers.All(player => player.TurnEnded);
        var shouldAdvance = trigger == TurnPhaseAdvanceTrigger.Timeout
            ? !everyoneEnded
            : everyoneEnded;

        if (!shouldAdvance)
            return TurnPhaseTransitionResult.NoChange(context.Turn);

        context.Turn.EndedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.Turn);

        var nextTurn = await context.UnitOfWork.Turns.NextTurnAsync(context.Turn);
        nextTurn.ActivePlayerId = null;
        nextTurn.Phase = TurnPhase.Draw;
        nextTurn.StartedAt = DateTime.UtcNow;

        ResetTurnEndedFlags(players, context);

        return new TurnPhaseTransitionResult(nextTurn, ActivePlayerId: null, TurnChanged: true, PhaseChanged: true);
    }

    private static void MarkActorAsEndedIfNeeded(PlayerGame? actor, TurnPhaseStateContext context)
    {
        if (actor is null)
            throw new BadRequestException("Actor is required for this phase transition.");

        if (actor.TurnEnded)
            return;

        actor.TurnEnded = true;
        context.UnitOfWork.PlayerGames.Update(actor);
    }

    private static void ResetTurnEndedFlags(IEnumerable<PlayerGame> players, TurnPhaseStateContext context)
    {
        foreach (var player in players)
        {
            player.TurnEnded = player.LifePoints <= 0;
            context.UnitOfWork.PlayerGames.Update(player);
        }
    }
}
