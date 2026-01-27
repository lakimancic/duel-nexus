using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models;

public class AttackAction
{
    [Key]
    public Guid Id { get; set; }

    public Guid TurnId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; } = null!;

    public Guid AttackerCardId { get; set; }
    [ForeignKey(nameof(AttackerCardId))]
    public GameCard Attacker { get; set; } = null!;

    public Guid? DefenderCardId { get; set; }
    [ForeignKey(nameof(DefenderCardId))]
    public GameCard? Defender { get; set; }

    public DateTime ExecutedAt { get; set; }
}
