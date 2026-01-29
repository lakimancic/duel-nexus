namespace Backend.Application.DTOs.GameRooms;

public class GameRoomDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsRanked { get; set; }
    public string? JoinCode { get; set; }
    public IEnumerable<GameRoomPlayerDto> Players { get; set; } = [];
}
