using Backend.Application.DTOs.GameRooms;
using Backend.Data.Enums;

namespace Backend.Application.Services.Interfaces;

public interface IGameRoomService
{
    Task<List<GameRoomDto>> GetGameRoomsAsync(int page, int pageSize, RoomStatus? status);
    Task<GameRoomDto> CreateGameRoomAsync(Guid userId);
    Task<GameRoomDto> GetGameRoomByIdAsync(Guid gameRoomId);
    Task AddPlayerToGameRoomAsync(Guid gameRoomId, Guid userId);
    Task RemovePlayerFromGameRoomAsync(Guid gameRoomId, Guid userId);
    Task CancelGameRoomAsync(Guid gameRoomId);
    Task UpdatePlayerDeckAsync(Guid gameRoomId, Guid playerId, Guid? deckId);
}
