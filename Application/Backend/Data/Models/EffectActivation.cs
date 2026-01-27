using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models;

public class EffectActivation
{
    [Key]
    public Guid Id { get; set; }

    public Guid GameId { get; set; }
    [ForeignKey(nameof(GameId))]
    public Game Game { get; set; } = null!;
    public Guid TurnId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; } = null!;
    public Guid PlayerGameId { get; set; }
    [ForeignKey(nameof(PlayerGameId))]
    public PlayerGame PlayerGame { get; set; } = null!;

    public Guid EffectId { get; set; }
    [ForeignKey(nameof(EffectId))]
    public Effect Effect { get; set; } = null!;

    public Guid SourceCardId { get; set; }
    [ForeignKey(nameof(SourceCardId))]
    public GameCard SourceCard { get; set; } = null!;

    public DateTime ActivatedAt { get; set; }

    public bool Resolved { get; set; }
}
