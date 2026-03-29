namespace Backend.Domain.Engine.Phases;

using Backend.Data.Models;

public sealed record TurnPhaseTransitionResult(
    Turn Turn,
    Guid? ActivePlayerId,
    bool TurnChanged,
    bool PhaseChanged)
{
    public static TurnPhaseTransitionResult NoChange(Turn turn)
        => new(turn, turn.ActivePlayerId, TurnChanged: false, PhaseChanged: false);
}
