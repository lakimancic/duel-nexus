using Backend.Data.Enums;

namespace Backend.Data.Models;

public class SpellCard : Card
{
    public SpellCard()
    {
        Type = CardType.Spell;
    }
}
