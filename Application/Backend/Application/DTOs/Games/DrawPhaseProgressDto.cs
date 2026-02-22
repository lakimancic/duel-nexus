using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class DrawPhaseProgressDto
{
    public Guid GameId { get; set; }
    public Guid RoomId { get; set; }
    public Guid PlayerGameId { get; set; }
    public Guid TurnId { get; set; }
    public bool TurnEnded { get; set; }
    public bool PhaseAdvanced { get; set; }
    public TurnPhase CurrentPhase { get; set; }
}
