namespace Backend.Domain.Engine;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed record DrawPhaseProgressResult(
    Game Game,
    Turn Turn,
    PlayerGame Player,
    bool TurnEnded,
    bool PhaseAdvanced,
    TurnPhase CurrentPhase
);
