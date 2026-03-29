namespace Backend.Domain.Engine;

using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Domain.Engine.Phases;

public sealed record GameCommandContext(
    IUnitOfWork UnitOfWork,
    Game Game,
    Turn CurrentTurn,
    PlayerGame Actor,
    ITurnPhaseStateMachine PhaseStateMachine
);
