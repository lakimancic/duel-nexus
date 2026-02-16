using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameRoomRepository : IRepository<GameRoom>
{
    Task<GameRoom?> GetWithHostById(Guid id);
    Task<GameRoom?> GetFriendlyWaitingByJoinCode(string joinCode);
}
