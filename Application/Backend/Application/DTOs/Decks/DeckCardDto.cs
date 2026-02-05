using Backend.Application.DTOs.Cards;

namespace Backend.Application.DTOs.Decks;

public class DeckCardDto
{
    public CardDto Card { get; set; } = null!;
    public int Quantity { get; set; }
}
