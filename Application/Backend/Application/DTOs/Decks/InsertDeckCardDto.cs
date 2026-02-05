namespace Backend.Application.DTOs.Decks;

public class InsertDeckCardDto
{
    public Guid PlayerCardId { get; set; }
    public int Quantity { get; set; }
}
