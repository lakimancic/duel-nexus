using Backend.Data.Enums;

namespace Backend.Data.Models;

public class MonsterCard : Card
{
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Level { get; set; }

    public MonsterCard()
    {
        Type = CardType.Monster;
    }
}
