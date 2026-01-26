using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public abstract class Card
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string Description { get; set; } = null!;
    public CardType Type { get; protected set; }

    public Guid? EffectId { get; set; }
    [ForeignKey(nameof(EffectId))]
    public Effect? Effect { get; set; }
}
