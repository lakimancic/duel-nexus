using System.ComponentModel.DataAnnotations;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class Effect
{
    [Key]
    public Guid Id { get; set; }

    public int Affects { get; set; }
    public int Points { get; set; }
    public int Turns { get; set; }
    public EffectType Type { get; set; }
}
