namespace Backend.Application.DTOs.Games;

public class GameStartedEventDto
{
    public Guid RoomId { get; set; }
    public Guid GameId { get; set; }
}
