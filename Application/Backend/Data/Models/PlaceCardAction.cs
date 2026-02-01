using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Models;

[Index(nameof(TurnId), nameof(GameCardId), IsUnique = true)]
public class PlaceCardAction
{
    [Key]
    public Guid Id { get; set; }

    public Guid TurnId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; } = null!;

    public Guid GameCardId { get; set; }
    [ForeignKey(nameof(GameCardId))]
    public GameCard Card { get; set; } = null!;

    public int? FieldIndex { get; set; }

    public bool FaceDown { get; set; }

    public bool DefensePosition { get; set; }

    public PlaceType Type { get; set; }

    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}
