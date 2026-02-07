using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class EditGameCardDto
{
    public CardZone Zone { get; set; }
    public int? DeckOrder { get; set; }
    public bool IsFaceDown { get; set; }
    public int? FieldIndex { get; set; }
    public bool DefensePosition { get; set; }
}
