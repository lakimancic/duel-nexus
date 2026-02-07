using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameRoomRepository(DuelNexusDbContext context) : Repository<GameRoom>(context), IGameRoomRepository
{
    public Task<GameRoom?> GetWithHostById(Guid id)
    {
        return _dbSet
            .Where(gr => gr.Id == id)
            .Include(gr => gr.HostUser)
            .FirstOrDefaultAsync();
    }
}
