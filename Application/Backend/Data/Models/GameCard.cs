using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class GameCard
{
    [Key]
    public Guid Id { get; set; }

    public Guid PlayerGameId { get; set; }
    [ForeignKey(nameof(PlayerGameId))]
    public PlayerGame PlayerGame { get; set; } = null!;

    public Guid CardId { get; set; }
    [ForeignKey(nameof(CardId))]
    public Card Card { get; set; } = null!;

    public CardZone Zone { get; set; }

    public int? DeckOrder { get; set; }

    public bool IsFaceDown { get; set; }

    public int? FieldIndex { get; set; }

    public bool DefensePosition { get; set; }
}
