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

    public Task<GameRoom?> GetFriendlyWaitingByJoinCode(string joinCode)
    {
        return _dbSet
            .Where(gr =>
                !gr.IsRanked &&
                gr.Status == Backend.Data.Enums.RoomStatus.Waiting &&
                gr.JoinCode != null &&
                gr.JoinCode == joinCode)
            .Include(gr => gr.HostUser)
            .FirstOrDefaultAsync();
    }
}
