namespace Backend.Application.DTOs.GameRooms;

public class GameRoomPlayerDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public double Elo { get; set; }
    public bool IsReady { get; set; }
}
