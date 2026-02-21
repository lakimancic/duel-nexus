using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Models;

[Index(nameof(GameId), nameof(UserId), IsUnique = true)]
public class PlayerGame
{
    [Key]
    public Guid Id { get; set; }

    public int Index { get; set; }

    public Guid GameId { get; set; }
    [ForeignKey(nameof(GameId))]
    public Game Game { get; set; } = null!;

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public int LifePoints { get; set; } = 8000;

    public bool TurnEnded { get; set; } = false;

    public ICollection<GameCard> Cards { get; set; } = [];
}
