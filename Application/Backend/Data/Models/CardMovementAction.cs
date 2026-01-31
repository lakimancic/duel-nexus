using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Models;

[Index(nameof(TurnId), nameof(GameCardId), IsUnique = true)]
public class CardMovementAction
{
    [Key]
    public Guid Id { get; set; }

    public Guid TurnId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; } = null!;

    public Guid GameCardId { get; set; }
    [ForeignKey(nameof(GameCardId))]
    public GameCard Card { get; set; } = null!;

    public CardZone FromZone { get; set; }
    public CardZone ToZone { get; set; }

    public CardMovementType MovementType { get; set; }

    public DateTime ExecutedAt { get; set; }
}
