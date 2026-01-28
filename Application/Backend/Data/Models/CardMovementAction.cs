using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class CardMovementAction
{
    [Key]
    public Guid Id { get; set; }

    public Guid TurnId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; } = null!;

    public Guid PlayerGameId { get; set; }
    [ForeignKey(nameof(PlayerGameId))]
    public PlayerGame Player { get; set; } = null!;

    public Guid GameCardId { get; set; }
    [ForeignKey(nameof(GameCardId))]
    public GameCard Card { get; set; } = null!;

    public CardZone FromZone { get; set; }
    public CardZone ToZone { get; set; }

    public CardMovementType MovementType { get; set; }

    public DateTime ExecutedAt { get; set; }
}
