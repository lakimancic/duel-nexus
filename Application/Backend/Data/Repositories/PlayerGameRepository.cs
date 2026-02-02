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
            DeckId = grp.DeckId!.Value,
            Game = game,
            Index = index,
        };
        await AddAsync(playerGame);
        return playerGame;
    }

    public Task<PlayerGame?> GetByUserIdAndGameIdAsync(Guid userId, Guid gameId)
    {
        return _context.PlayerGames
            .Where(pg => pg.UserId == userId && pg.GameId == gameId)
            .FirstOrDefaultAsync();
    }
}
