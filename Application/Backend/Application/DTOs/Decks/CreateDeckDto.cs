namespace Backend.Application.DTOs.Decks;

public class CreateDeckDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsComplete { get; set; }
}
