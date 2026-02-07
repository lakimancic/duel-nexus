namespace Backend.Application.DTOs.Games;

public class GameDto
{
    public Guid Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public Guid RoomId { get; set; }
}
