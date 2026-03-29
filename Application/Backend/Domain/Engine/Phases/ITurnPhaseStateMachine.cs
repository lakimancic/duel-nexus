namespace Backend.Domain.Engine.Phases;

using Backend.Data.Enums;

public interface ITurnPhaseStateMachine
{
    ITurnPhaseState GetState(TurnPhase phase);

    Task<TurnPhaseTransitionResult> AdvanceAsync(
        TurnPhaseStateContext context,
        TurnPhaseAdvanceTrigger trigger,
        CancellationToken cancellationToken = default);

    Task<TurnPhaseTransitionResult> TryAdvanceOnTimeoutAsync(
        TurnPhaseStateContext context,
        CancellationToken cancellationToken = default);
}
