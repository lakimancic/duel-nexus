using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameRoomPlayerRepository(DuelNexusDbContext context) : Repository<GameRoomPlayer>(context), IGameRoomPlayerRepository
{
    public async Task<List<GameRoomPlayer>> GetByGameRoomId(Guid id)
    {
        return await _dbSet
            .Where(grp => grp.GameRoomId == id)
            .Include(grp => grp.Deck)
            .Include(grp => grp.User)
            .ToListAsync();
    }

    public async Task<GameRoomPlayer?> GetByRoomAndUserId(Guid id, Guid userId)
    {
        return await _dbSet
            .Where(grp => grp.GameRoomId == id && grp.UserId == userId)
            .Include(grp => grp.User)
            .FirstOrDefaultAsync();
    }
}
