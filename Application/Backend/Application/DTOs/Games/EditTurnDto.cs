using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class EditTurnDto
{
    public int TurnNumber { get; set; }
    public Guid ActivePlayerId { get; set; }
    public TurnPhase Phase { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
}
