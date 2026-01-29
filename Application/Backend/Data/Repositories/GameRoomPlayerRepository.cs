using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameRoomPlayerRepository(DuelNexusDbContext context) : Repository<GameRoomPlayer>(context), IGameRoomPlayerRepository
{
    public async Task<GameRoomPlayer?> GetByGameRoomIdAndPlayerIdAsync(Guid gameRoomId, Guid playerId)
    {
        return await _dbSet.FirstOrDefaultAsync(grp => grp.GameRoomId == gameRoomId && grp.UserId == playerId);
    }
}
