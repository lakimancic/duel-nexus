using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models;

public class DeckCard
{
    [Key]
    public Guid Id { get; set; }

    public Guid DeckId { get; set; }
    [ForeignKey(nameof(DeckId))]
    public Deck Deck { get; set; } = null!;

    public Guid CardId { get; set; }
    [ForeignKey(nameof(CardId))]
    public Card Card { get; set; } = null!;

    public int Quantity { get; set; }
}
