using Backend.Application.DTOs.Cards;

namespace Backend.Application.DTOs.Users;

public class PlayerCardDto
{
    public Guid Id { get; set; }
    public CardDto Card { get; set; } = null!;
    public int Quantity { get; set; }
}
