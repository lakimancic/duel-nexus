namespace Backend.Application.DTOs.Decks;

public class EditDeckCardQuantityDto
{
    public Guid CardId { get; set; }
    public int Quantity { get; set; }
}
