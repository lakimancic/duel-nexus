using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class Deck
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ICollection<DeckCard> Cards { get; set; } = [];
}
