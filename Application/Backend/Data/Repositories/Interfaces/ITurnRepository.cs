using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface ITurnRepository : IRepository<Turn>
{
    Task<Turn?> GetCurrentTurnAsync(Guid gameId);
    Task InitializeTurnsForGameAsync(Game game);
    Task NextTurnAsync(Turn turn);
}
