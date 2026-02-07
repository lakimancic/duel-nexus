namespace Backend.Application.DTOs.Games;

public class CreateEffectActivationDto
{
    public Guid TurnId { get; set; }
    public Guid EffectId { get; set; }
    public Guid SourceCardId { get; set; }
    public bool Resolved { get; set; }
}
