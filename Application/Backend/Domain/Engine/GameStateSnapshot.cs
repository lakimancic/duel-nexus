namespace Backend.Domain.Engine;

using Backend.Data.Models;

public sealed record GameStateSnapshot(
    Game Game,
    Turn CurrentTurn,
    PlayerGame Viewer,
    IReadOnlyList<PlayerGame> Players,
    IReadOnlyList<GameCard> Cards
);
