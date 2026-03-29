namespace Backend.Domain.Commands;

using Backend.Domain.Engine;
using Backend.Domain.Engine.Phases;

public sealed record AdvancePhaseActionCommand : IGameCommand<PhaseAdvanceResult>
{
    public async Task<PhaseAdvanceResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var transition = await context.PhaseStateMachine.AdvanceAsync(
            new TurnPhaseStateContext(context.UnitOfWork, context.Game, context.CurrentTurn, context.Actor),
            TurnPhaseAdvanceTrigger.PlayerAdvance,
            cancellationToken);

        return new PhaseAdvanceResult(
            Game: context.Game,
            Turn: transition.Turn,
            Player: context.Actor,
            ActivePlayerId: transition.ActivePlayerId,
            CurrentPhase: transition.Turn.Phase,
            TurnChanged: transition.TurnChanged
        );
    }
}
