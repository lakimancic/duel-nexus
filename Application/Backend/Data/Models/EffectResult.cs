using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class EffectResult
{
    [Key]
    public Guid Id { get; set; }

    public Guid ActivationId { get; set; }
    [ForeignKey(nameof(ActivationId))]
    public EffectActivation Activation { get; set; } = null!;

    public Guid? AffectedCardId { get; set; }
    [ForeignKey(nameof(AffectedCardId))]
    public GameCard AffectedCard { get; set; } = null!;
    public Guid? AffectedPlayerId { get; set; }
    [ForeignKey(nameof(AffectedPlayerId))]
    public PlayerGame AffectedPlayer { get; set; } = null!;

    public CardZone? PreviousZone { get; set; }
    public CardZone? NewZone { get; set; }

    public int? LifePointsChange { get; set; }
}
