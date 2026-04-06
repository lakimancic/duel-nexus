namespace Backend.Application.DTOs.Games;

using Backend.Data.Enums;

public class BattleAttackResultDto
{
    public Guid GameId { get; set; }
    public Guid RoomId { get; set; }
    public Guid TurnId { get; set; }
    public Guid PlayerGameId { get; set; }
    public Guid AttackerCardId { get; set; }
    public Guid? DefenderCardId { get; set; }
    public Guid? DefenderPlayerGameId { get; set; }
    public int DamageToDefender { get; set; }
    public int DamageToAttacker { get; set; }
    public bool AttackerDestroyed { get; set; }
    public bool DefenderDestroyed { get; set; }
    public bool AttackFailed { get; set; }
    public bool PhaseAdvanced { get; set; }
    public bool TurnChanged { get; set; }
    public Guid? ActivePlayerId { get; set; }
    public TurnPhase CurrentPhase { get; set; }
}
