using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IPlayerGameRepository : IRepository<PlayerGame>
{
    Task<PlayerGame> CreatePlayerAsync(GameRoomPlayer grp, Game game, int index);
    Task<PlayerGame?> GetByUserIdAndGameIdAsync(Guid userId, Guid gameId);
}
