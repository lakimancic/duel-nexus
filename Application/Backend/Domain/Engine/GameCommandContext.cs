namespace Backend.Domain.Engine;

using Backend.Data.Models;
using Backend.Data.UnitOfWork;

public sealed record GameCommandContext(
    IUnitOfWork UnitOfWork,
    Game Game,
    Turn CurrentTurn,
    PlayerGame Actor
);
