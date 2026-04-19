namespace Backend.Application.DTOs.Games;

using Backend.Data.Enums;

public class TrapCardOptionDto
{
    public Guid GameCardId { get; set; }
    public string CardName { get; set; } = string.Empty;
    public Guid? EffectId { get; set; }
    public EffectType? EffectType { get; set; }
    public bool RequiresTarget { get; set; }
    public bool TargetsPlayer { get; set; }
    public int? Affects { get; set; }
}
