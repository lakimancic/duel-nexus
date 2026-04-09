namespace Backend.Application.DTOs.Games;

using Backend.Data.Enums;

public class TrapCardOptionDto
{
    public Guid GameCardId { get; set; }
    public string CardName { get; set; } = string.Empty;
    public Guid? EffectId { get; set; }
    public EffectType? EffectType { get; set; }
}
