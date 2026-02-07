using Backend.Data.Enums;

namespace Backend.Application.DTOs.GameRooms;

public class EditGameRoomDto
{
    public bool IsRanked { get; set; }
    public RoomStatus Status { get; set; }
    public string? JoinCode { get; set; }
    public DateTime CreatedAt { get; set; }
}
