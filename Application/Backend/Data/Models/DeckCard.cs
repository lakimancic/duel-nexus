using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class DeckCard
{
    [Key]
    public Guid Id { get; set; }

    public Guid DeckId { get; set; }
    public Deck Deck { get; set; } = null!;

    public Guid CardId { get; set; }
    public Card Card { get; set; } = null!;

    public int Quantity { get; set; }
}
