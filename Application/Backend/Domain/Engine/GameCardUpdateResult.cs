namespace Backend.Domain.Engine;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed record GameCardUpdateResult(
    Game Game,
    Turn Turn,
    PlayerGame Player,
    GameCard Card,
    TurnPhase CurrentPhase
);
