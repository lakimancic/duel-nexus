using Backend.Data.Enums;

namespace Backend.Application.DTOs.Effects;

public class EffectDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;

    public int? Affects { get; set; }
    public int? Points { get; set; }
    public int? Turns { get; set; }
    public bool RequiresTarget { get; set; }
    public bool TargetsPlayer { get; set; }
}
