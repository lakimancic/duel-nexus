using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class PlayerGame
{
    [Key]
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid DeckId { get; set; }
    public Deck DeckTemplate { get; set; } = null!;

    public int LifePoints { get; set; } = 8000;

    public ICollection<GameCard> Cards { get; set; } = [];
}
