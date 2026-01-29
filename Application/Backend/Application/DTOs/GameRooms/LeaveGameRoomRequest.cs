namespace Backend.Application.DTOs.GameRooms;

public class LeaveGameRoomRequest
{
    public Guid GameRoomId { get; set; }
    public Guid UserId { get; set; }
}
