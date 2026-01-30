using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.GameRooms;

public class JoinGameRoomRequest
{
    [Required(ErrorMessage = "Code is required")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Code must be exactly 6 characters")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "UserId is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "DeckId is required")]
    public Guid DeckId { get; set; }
}
