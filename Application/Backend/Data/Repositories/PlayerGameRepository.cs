using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class PlayerGameRepository(DuelNexusDbContext context) : Repository<PlayerGame>(context), IPlayerGameRepository
{
    public async Task<PlayerGame> CreatePlayerAsync(GameRoomPlayer grp, Game game, int index)
    {
        var playerGame = new PlayerGame
        {
            UserId = grp.UserId,
            Game = game,
            Index = index,
        };
        await AddAsync(playerGame);
        return playerGame;
    }

    public Task<PlayerGame?> GetByUserIdAndGameIdAsync(Guid userId, Guid gameId)
    {
        return _dbSet
            .Where(pg => pg.UserId == userId && pg.GameId == gameId)
            .FirstOrDefaultAsync();
    }

    public Task<List<PlayerGame>> GetByGameIdOrderedAsync(Guid gameId)
    {
        return _dbSet
            .Where(pg => pg.GameId == gameId)
            .OrderBy(pg => pg.Index)
            .ToListAsync();
    }

    public Task<PlayerGame?> GetByWithUserById(Guid id)
    {
        return _dbSet
            .Where(pg => pg.Id == id)
            .Include(pg => pg.User)
            .FirstOrDefaultAsync();
    }

    public Task<List<PlayerGame>> GetByGameIdWithUserAsync(Guid gameId)
    {
        return _dbSet
            .Where(pg => pg.GameId == gameId)
            .OrderBy(pg => pg.Index)
            .Include(pg => pg.User)
            .ToListAsync();
    }
}
