namespace Backend.Domain.Commands;

using Backend.Domain.Engine;
using Backend.Domain.Engine.Phases;

public sealed record SkipDrawActionCommand : IGameCommand<DrawPhaseProgressResult>
{
    public async Task<DrawPhaseProgressResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (!context.Actor.TurnEnded)
        {
            context.Actor.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(context.Actor);
        }

        var transition = await context.PhaseStateMachine.AdvanceAsync(
            new TurnPhaseStateContext(context.UnitOfWork, context.Game, context.CurrentTurn, context.Actor),
            TurnPhaseAdvanceTrigger.PlayerCompletedActions,
            cancellationToken);

        return new DrawPhaseProgressResult(
            Game: context.Game,
            Turn: transition.Turn,
            Player: context.Actor,
            TurnEnded: context.Actor.TurnEnded,
            PhaseAdvanced: transition.PhaseChanged || transition.TurnChanged,
            CurrentPhase: transition.Turn.Phase
        );
    }
}
