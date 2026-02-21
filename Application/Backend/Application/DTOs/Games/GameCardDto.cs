using Backend.Application.DTOs.Cards;
using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class GameCardDto
{
    public Guid Id { get; set; }
    public CardZone Zone { get; set; }
    public int? DeckOrder { get; set; }
    public bool IsFaceDown { get; set; }
    public int? FieldIndex { get; set; }
    public bool DefensePosition { get; set; }
    public CardDto? Card { get; set; }
    public Guid PlayerGameId { get; set; }
}
