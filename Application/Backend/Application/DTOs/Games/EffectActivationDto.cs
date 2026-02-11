namespace Backend.Application.DTOs.Games;

public class EffectActivationDto
{
    public Guid Id { get; set; }
    public ShortTurnDto Turn { get; set; } = null!;
    public Guid EffectId { get; set; }
    public Guid SourceCardId { get; set; }
    public bool Resolved { get; set; }
}
