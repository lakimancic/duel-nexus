using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameRoomRepository : IRepository<GameRoom>
{
    Task<GameRoom?> GetByCodeAsync(string code);
    Task<GameRoom> CreateGameRoomByHostAsync(User host, Deck deck);
    Task<List<GameRoom>> GetAllWithPlayersAsync();
    Task<GameRoom?> GetByIdWithPlayersAsync(Guid id);
}
