namespace Backend.Application.DTOs.Games;

public class EffectActivationDto
{
    public Guid Id { get; set; }
    public ShortTurnDto Turn { get; set; } = null!;
    public Guid EffectId { get; set; }
    public GameCardDto SourceCardId { get; set; } = null!;
    public bool Resolved { get; set; }
}
