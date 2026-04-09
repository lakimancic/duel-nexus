namespace Backend.Application.DTOs.Games;

using Backend.Data.Enums;

public class GameEffectActivationDto
{
    public Guid SourceCardId { get; set; }
    public Guid ActivatedByPlayerGameId { get; set; }
    public string SourceCardName { get; set; } = string.Empty;
    public EffectType EffectType { get; set; }
    public bool IsTrap { get; set; }
}
