namespace Backend.Application.DTOs.Decks;

public class EditDeckDto
{
    public string Name { get; set; } = null!;
    public bool IsComplete { get; set; }
}
