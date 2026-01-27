using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models;

public class EffectTarget
{
    [Key]
    public Guid Id { get; set; }

    public Guid ActivationId { get; set; }
    [ForeignKey(nameof(ActivationId))]
    public EffectActivation Activation { get; set; } = null!;

    public Guid? TargetCardId { get; set; }
    [ForeignKey(nameof(TargetCardId))]
    public GameCard? TargetCard { get; set; }

    public Guid? TargetPlayerId { get; set; }
    [ForeignKey(nameof(TargetPlayerId))]
    public PlayerGame TargetPlayer { get; set; } = null!;
}
