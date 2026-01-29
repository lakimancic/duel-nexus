namespace Backend.Application.DTOs.GameRooms;

public class JoinGameRoomRequest
{
    public string Code { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid DeckId { get; set; }
}
