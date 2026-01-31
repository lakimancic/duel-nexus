using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Models;

[Index(nameof(UserId), nameof(Name), IsUnique = true)]
public class Deck
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsComplete { get; set; }

    public ICollection<DeckCard> Cards { get; set; } = [];
}
