namespace Backend.Domain.Engine;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed record PlaceCardResult(
    Game Game,
    Turn Turn,
    PlayerGame Player,
    GameCard Card,
    int FieldIndex,
    bool FaceDown,
    TurnPhase CurrentPhase
);
