using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class Turn
{
    [Key]
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    [ForeignKey(nameof(GameId))]
    public Game Game { get; set; } = null!;

    public int TurnNumber { get; set; }

    public Guid ActivePlayerId { get; set; }
    [ForeignKey(nameof(ActivePlayerId))]
    public PlayerGame ActivePlayer { get; set; } = null!;

    public TurnPhase Phase { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}
