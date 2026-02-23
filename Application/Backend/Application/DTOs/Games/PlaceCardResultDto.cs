using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class PlaceCardResultDto
{
    public Guid GameId { get; set; }
    public Guid RoomId { get; set; }
    public Guid PlayerGameId { get; set; }
    public Guid TurnId { get; set; }
    public Guid GameCardId { get; set; }
    public int FieldIndex { get; set; }
    public bool FaceDown { get; set; }
    public TurnPhase CurrentPhase { get; set; }
}
