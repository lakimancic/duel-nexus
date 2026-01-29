namespace Backend.Application.DTOs.GameRooms;

public class CreateGameRoomRequest
{
    public Guid UserId { get; set; }
    public Guid DeckId { get; set; }
}
