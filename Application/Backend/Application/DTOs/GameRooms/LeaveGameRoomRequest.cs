using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.GameRooms;

public class LeaveGameRoomRequest
{
    [Required(ErrorMessage = "GameRoomId is required")]
    public Guid GameRoomId { get; set; }

    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; set; }
}
