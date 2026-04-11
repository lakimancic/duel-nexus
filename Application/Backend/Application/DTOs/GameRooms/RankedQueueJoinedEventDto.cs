namespace Backend.Application.DTOs.GameRooms;

public class RankedQueueJoinedEventDto
{
    public Guid UserId { get; set; }
    public int Position { get; set; }
    public int QueueSize { get; set; }
    public DateTime QueuedAt { get; set; }
    public double Elo { get; set; }
}
