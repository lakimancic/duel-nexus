namespace Backend.Application.DTOs.Decks;

public class DeckDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
}
