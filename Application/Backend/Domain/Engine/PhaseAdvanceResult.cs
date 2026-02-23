namespace Backend.Domain.Engine;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed record PhaseAdvanceResult(
    Game Game,
    Turn Turn,
    PlayerGame Player,
    Guid ActivePlayerId,
    TurnPhase CurrentPhase,
    bool TurnChanged
);
