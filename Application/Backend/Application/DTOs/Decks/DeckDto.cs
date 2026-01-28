public class DeckDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    
    public string Name { get; set; } = null!;

    public ICollection<DeckCardDto> Cards { get; set; } = [];
}