using Backend.Application.DTOs.Users;

namespace Backend.Application.DTOs.Decks;

public class DeckDto
{
    public Guid Id { get; set; }
    public ShortUserDto User {get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsComplete { get; set; }
}
