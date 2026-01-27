using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class CardField
{
    [Key]
    public Guid Id { get; set; }

    public Guid PlayerGameId { get; set; }
    [ForeignKey(nameof(PlayerGameId))]
    public PlayerGame PlayerGame { get; set; } = null!;

    public int Index { get; set; }

    public Guid? GameCardId { get; set; }
    [ForeignKey(nameof(GameCardId))]
    public GameCard? Card { get; set; }

    public CardPosition Position { get; set; }
    public bool IsFaceDown { get; set; }
}
