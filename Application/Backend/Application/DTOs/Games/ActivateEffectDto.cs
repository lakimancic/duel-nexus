namespace Backend.Application.DTOs.Games;

public class ActivateEffectDto
{
    public Guid CardId { get; set; }
    public List<EffectTargetDto> Targets { get; set; } = [];
}
