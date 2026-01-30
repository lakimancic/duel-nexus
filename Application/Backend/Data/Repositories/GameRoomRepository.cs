using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameRoomRepository(DuelNexusDbContext context) : Repository<GameRoom>(context), IGameRoomRepository
{
    public async Task<GameRoom> CreateGameRoomByHostAsync(User host, Deck deck)
    {
        if (host == null)
            throw new ArgumentNullException(nameof(host), "Host cannot be null");
        if (deck == null)
            throw new ArgumentNullException(nameof(deck), "Deck cannot be null");
        if (host.Id == Guid.Empty)
            throw new ArgumentException("Host ID cannot be empty", nameof(host));
        if (deck.Id == Guid.Empty)
            throw new ArgumentException("Deck ID cannot be empty", nameof(deck));

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
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty or whitespace", nameof(code));

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
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty", nameof(id));

        return await _dbSet
            .Include(gr => gr.Players)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(gr => gr.Id == id);
    }
}
