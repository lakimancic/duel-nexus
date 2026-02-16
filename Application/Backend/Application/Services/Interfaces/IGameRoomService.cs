using Backend.Application.DTOs.GameRooms;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IGameRoomService
{
    Task<GameRoomPlayerDto> AddPlayerToGameRoom(Guid id, InsertGameRoomPlayerDto playerDto);
    Task<GameRoomDto> CreateRoom(CreateGameRoomDto gameRoomDto);
    Task<GameRoomDto> CreateFriendlyRoom(Guid hostUserId);
    Task<GameRoomDto> JoinFriendlyRoom(Guid userId, string joinCode);
    Task DeleteGameRoomPlayer(Guid id, Guid userId);
    Task<bool> LeaveRoom(Guid id, Guid userId);
    Task CancelRoom(Guid id, Guid hostUserId);
    Task<Guid> StartRoomGame(Guid id, Guid hostUserId);
    Task<GameRoomDto> EditGameRoom(Guid id, EditGameRoomDto gameRoomDto);
    Task<GameRoomPlayerDto> SetPlayerGameDeck(Guid id, Guid userId, Guid deckId);
    Task<GameRoomDto?> GetGameRoomById(Guid id);
    Task<List<GameRoomPlayerDto>> GetGameRoomPlayers(Guid id);
    Task<PagedResult<GameRoomDto>> GetRooms(int page, int pageSize);
    Task<bool> UserExistsInRoom(Guid id, Guid userId);
    Task<bool> HasAtLeastOneCompleteDeck(Guid userId);
}
