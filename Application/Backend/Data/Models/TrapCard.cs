using Backend.Data.Enums;

namespace Backend.Data.Models;

public class TrapCard : Card
{
    public TrapCard()
    {
        Type = CardType.Trap;
    }
}
