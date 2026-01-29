using Backend.Application.DTOs.GameRooms;
using Backend.Data.Models;

namespace Backend.Application.Services.Interfaces;

public interface IGameRoomService
{
    Task<GameRoomDto> CreateGameRoomAsync(Guid userId, Guid deckId);
    Task<IEnumerable<GameRoomDto>> GetAllGameRoomsAsync();
    Task<Guid> JoinGameRoomAsync(string code, Guid userId, Guid deckId);
    Task LeaveGameRoomAsync(Guid gameRoomId, Guid userId);
    Task<GameRoomDto?> GetGameRoomByIdAsync(Guid gameRoomId);
}
