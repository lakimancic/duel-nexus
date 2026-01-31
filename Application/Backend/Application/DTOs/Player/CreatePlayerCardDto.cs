namespace Backend.Application.DTOs.Player;

public class CreatePlayerCardDto
{
    public Guid DeckId { get; set; }
    public Guid CardId { get; set; }
    public int Quantity { get; set; }
}
