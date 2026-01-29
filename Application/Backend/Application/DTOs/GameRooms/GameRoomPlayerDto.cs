namespace Backend.Application.DTOs.GameRooms;

public class GameRoomPlayerDto
{
    public string UserName { get; set; } = null!;
    public double UserElo { get; set; }
    public bool IsReady { get; set; }
}
