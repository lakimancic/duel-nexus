namespace Backend.Domain.Engine.Phases;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Utils.WebApi;

public sealed class Main1PhaseState : ITurnPhaseState
{
    private static readonly TimeSpan Main1PhaseTimeout = TimeSpan.FromMinutes(1);

    public TurnPhase Phase => TurnPhase.Main1;
    public TimeSpan? Timeout => Main1PhaseTimeout;

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
                throw new BadRequestException("Unsupported trigger for Main1 phase.");
        }

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        var everyoneEnded = players.All(player => player.TurnEnded);
        var shouldAdvance = trigger == TurnPhaseAdvanceTrigger.Timeout
            ? !everyoneEnded
            : everyoneEnded;

        if (!shouldAdvance)
        {
            if (context.Turn.ActivePlayerId != null)
            {
                context.Turn.ActivePlayerId = null;
                context.UnitOfWork.Turns.Update(context.Turn);
            }

            return TurnPhaseTransitionResult.NoChange(context.Turn);
        }

        var battleStarter = players[(context.Turn.TurnNumber - 1) % players.Count];
        context.Turn.Phase = TurnPhase.Battle;
        context.Turn.ActivePlayerId = battleStarter.Id;
        context.Turn.StartedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.Turn);

        ResetTurnEndedFlags(players, context);

        return new TurnPhaseTransitionResult(context.Turn, ActivePlayerId: battleStarter.Id, TurnChanged: false, PhaseChanged: true);
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
            player.TurnEnded = false;
            context.UnitOfWork.PlayerGames.Update(player);
        }
    }
}
