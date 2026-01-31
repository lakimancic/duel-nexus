using Backend.Data.Enums;

public class CreateEffectDto
{
    public EffectType Type { get; set; }

    public int? Affects { get; set; }
    public int? Points { get; set; }
    public int? Turns { get; set; }
    public bool RequiresTarget { get; set; }
    public bool TargetsPlayer { get; set; }
}
