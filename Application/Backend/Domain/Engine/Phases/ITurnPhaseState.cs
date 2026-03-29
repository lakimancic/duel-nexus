namespace Backend.Domain.Engine.Phases;

using Backend.Data.Enums;

public interface ITurnPhaseState
{
    TurnPhase Phase { get; }
    TimeSpan? Timeout { get; }

    Task<TurnPhaseTransitionResult> AdvanceAsync(
        TurnPhaseStateContext context,
        TurnPhaseAdvanceTrigger trigger,
        CancellationToken cancellationToken = default);
}
