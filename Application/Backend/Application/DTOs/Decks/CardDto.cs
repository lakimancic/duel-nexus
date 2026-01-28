using Backend.Data.Enums;

public class CardDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string Description { get; set; } = null!;
    public CardType Type { get; protected set; }
    public Guid? EffectId { get; set; }

}