using Backend.Application.DTOs.Effects;
using Backend.Data.Enums;

namespace Backend.Application.DTOs.Decks;

public class CreateCardDto
{
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string Description { get; set; } = null!;
    public CardType Type { get; protected set; }
    public Guid? EffectId { get; set; }

    public int? Attack { get; set; }
    public int? Defense { get; set; }
    public int? Level { get; set; }

}
