namespace Backend.Application.DTOs.GameRooms;

public class CreateGameRoomDto
{
    public bool IsRanked { get; set; }
    public Guid HostUserId { get; set; }
}
