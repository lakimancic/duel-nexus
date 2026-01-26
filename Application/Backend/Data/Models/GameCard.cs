using System.ComponentModel.DataAnnotations;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class GameCard
{
    [Key]
    public Guid Id { get; set; }

    public Guid PlayerGameId { get; set; }
    public PlayerGame PlayerGame { get; set; } = null!;

    public Guid CardId { get; set; }
    public Card Card { get; set; } = null!;

    public CardZone Zone { get; set; }

    public int? DeckOrder { get; set; }

    public bool IsFaceDown { get; set; }
}
