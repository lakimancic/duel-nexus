using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class GameCardUpdateResultDto
{
    public Guid GameId { get; set; }
    public Guid RoomId { get; set; }
    public Guid PlayerGameId { get; set; }
    public Guid TurnId { get; set; }
    public Guid GameCardId { get; set; }
    public CardZone Zone { get; set; }
    public int? FieldIndex { get; set; }
    public bool IsFaceDown { get; set; }
    public bool DefensePosition { get; set; }
    public TurnPhase CurrentPhase { get; set; }
}
