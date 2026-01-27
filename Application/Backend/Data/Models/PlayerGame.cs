using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models;

public class PlayerGame
{
    [Key]
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    [ForeignKey(nameof(GameId))]
    public Game Game { get; set; } = null!;

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public Guid DeckId { get; set; }
    [ForeignKey(nameof(DeckId))]
    public Deck DeckTemplate { get; set; } = null!;

    public int LifePoints { get; set; } = 8000;

    public ICollection<GameCard> Cards { get; set; } = [];
}
