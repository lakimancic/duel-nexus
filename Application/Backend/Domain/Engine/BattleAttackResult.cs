namespace Backend.Domain.Engine;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed record BattleAttackResult(
    Game Game,
    Turn Turn,
    PlayerGame Player,
    Guid AttackerCardId,
    Guid? DefenderCardId,
    Guid? DefenderPlayerGameId,
    int DamageToDefender,
    int DamageToAttacker,
    bool AttackerDestroyed,
    bool DefenderDestroyed,
    bool AttackFailed,
    bool PhaseAdvanced,
    bool TurnChanged,
    Guid? ActivePlayerId,
    TurnPhase CurrentPhase
);
