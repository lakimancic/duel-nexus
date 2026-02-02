using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameRepository : IRepository<Game>
{
    Task<Game> CreateGameFromRoomAsync(GameRoom gameRoom);
}
