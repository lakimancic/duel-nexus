using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameRoomRepository(DuelNexusDbContext context) : Repository<GameRoom>(context), IGameRoomRepository
{
    public async Task<GameRoom> CreateGameRoomByHostAsync(User host, Deck deck)
    {
        var gameRoom = new GameRoom
        {
            HostUser = host,
            JoinCode = Guid.NewGuid().ToString()[..6].ToUpper(),
            CreatedAtUtc = DateTime.UtcNow,
            Status = Enums.RoomStatus.Waiting,
            Players = [
                new GameRoomPlayer
                {
                    User = host,
                    Deck = deck,
                }
            ]
        };
        await AddAsync(gameRoom);
        return gameRoom;
    }

    public async Task<GameRoom?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(gr => gr.Players)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(gr => gr.JoinCode == code);
    }

    public async Task<List<GameRoom>> GetAllWithPlayersAsync()
    {
        return await _dbSet
            .Include(gr => gr.Players)
                .ThenInclude(p => p.User)
            .ToListAsync();
    }

    public async Task<GameRoom?> GetByIdWithPlayersAsync(Guid id)
    {
        return await _dbSet
            .Include(gr => gr.Players)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(gr => gr.Id == id);
    }
}
