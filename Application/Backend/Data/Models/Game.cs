using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models;

public class Game
{
    [Key]
    public Guid Id { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public Guid RoomId { get; set; }
    [ForeignKey(nameof(RoomId))]
    public GameRoom Room { get; set; } = null!;

    public ICollection<PlayerGame> Players { get; set; } = [];
    public ICollection<Turn> Turns { get; set; } = [];
}
