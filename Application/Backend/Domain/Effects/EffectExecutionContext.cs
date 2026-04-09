namespace Backend.Domain.Effects;

using Backend.Data.Models;
using Backend.Data.UnitOfWork;

public sealed record EffectExecutionContext(
    IUnitOfWork UnitOfWork,
    Game Game,
    Turn Turn,
    PlayerGame ActivatingPlayer,
    GameCard SourceCard,
    bool IsTrapResponse,
    GameCard? AttackAttackerCard,
    PlayerGame? AttackAttackerPlayer,
    PlayerGame? AttackDefenderPlayer
);
