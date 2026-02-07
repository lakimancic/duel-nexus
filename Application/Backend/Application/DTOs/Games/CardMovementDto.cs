using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class CardMovementDto
{
    public Guid Id { get; set; }
    public ShortTurnDto Turn { get; set; } = null!;
    public GameCardDto GameCard { get; set; } = null!;
    public CardZone FromZone { get; set; }
    public CardZone ToZone { get; set; }
    public CardMovementType MovementType { get; set; }
    public DateTime ExecutedAt { get; set; }
}
