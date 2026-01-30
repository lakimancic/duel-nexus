using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.GameRooms;

public class CreateGameRoomRequest
{
    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "DeckId is required")]
    public Guid DeckId { get; set; }
}
