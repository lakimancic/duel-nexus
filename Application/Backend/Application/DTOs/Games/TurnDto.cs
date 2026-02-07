using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class TurnDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public int TurnNumber { get; set; }
    public Guid? ActivePlayerId { get; set; }
    public TurnPhase Phase { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}
