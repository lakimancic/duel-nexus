using Backend.Application.DTOs.Users;
using Backend.Data.Enums;

namespace Backend.Application.DTOs.GameRooms;

public class GameRoomDto
{
    public Guid Id { get; set; }
    public bool IsRanked { get; set; }
    public RoomStatus Status { get; set; }
    public string? JoinCode { get; set; }
    public Guid HostUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ShortUserDto HostUser { get; set; } = null!;
}
