namespace Backend.Domain.Engine;

using Backend.Data.Models;

public sealed record DrawActionResult(
    Game Game,
    Turn Turn,
    PlayerGame Player,
    GameCard DrawnCard,
    int DrawsInTurn,
    bool TurnEnded,
    Guid? NextActivePlayerId
);
