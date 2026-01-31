namespace Backend.Application.DTOs.Player;

public class PlayerCardDto
{
    public Guid Id { get; set; }
    public Guid DeckId { get; set; }
    public Guid CardId { get; set; }
    public int Quantity { get; set; }
}
