using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameRoomPlayerRepository : IRepository<GameRoomPlayer>
{
    Task<List<GameRoomPlayer>> GetByGameRoomId(Guid id);
    Task<GameRoomPlayer?> GetByRoomAndUserId(Guid id, Guid userId);
}
