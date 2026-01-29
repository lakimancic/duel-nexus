using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameRoomPlayerRepository : IRepository<GameRoomPlayer>
{
    Task<GameRoomPlayer?> GetByGameRoomIdAndPlayerIdAsync(Guid gameRoomId, Guid playerId);
}
