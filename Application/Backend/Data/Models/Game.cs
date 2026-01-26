using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class Game
{
    [Key]
    public Guid Id { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public ICollection<PlayerGame> Players { get; set; } = [];
}
