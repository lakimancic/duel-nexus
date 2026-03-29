namespace Backend.Domain.Engine.Phases;

using Backend.Data.Models;
using Backend.Data.UnitOfWork;

public sealed record TurnPhaseStateContext(
    IUnitOfWork UnitOfWork,
    Game Game,
    Turn Turn,
    PlayerGame? Actor = null
);
