using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Users;

namespace Backend.Application.DTOs.GameRooms;

public class GameRoomPlayerDto
{
    public Guid Id { get; set; }
    public ShortUserDto User { get; set; } = null!;
    public DeckDto? Deck { get; set; }
    public bool IsReady { get; set; }
}
