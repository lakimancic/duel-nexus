using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class PhaseAdvanceResultDto
{
    public Guid GameId { get; set; }
    public Guid RoomId { get; set; }
    public Guid TurnId { get; set; }
    public Guid ActivePlayerId { get; set; }
    public TurnPhase CurrentPhase { get; set; }
    public bool TurnChanged { get; set; }
}
