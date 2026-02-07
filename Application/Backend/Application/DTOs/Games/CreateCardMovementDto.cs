using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class CreateCardMovementDto
{
    public Guid TurnId { get; set; }
    public Guid GameCardId { get; set; }
    public CardZone FromZone { get; set; }
    public CardZone ToZone { get; set; }
    public CardMovementType MovementType { get; set; }
}
