using Backend.Application.DTOs.Effects;
using Backend.Data.Enums;

namespace Backend.Application.DTOs.Cards;

public class CardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string Description { get; set; } = null!;
    public CardType Type { get; protected set; }
    public Guid? EffectId { get; set; }
    public EffectDto? Effect { get; set; }

    public int? Attack { get; set; }
    public int? Defense { get; set; }
    public int? Level { get; set; }

}
