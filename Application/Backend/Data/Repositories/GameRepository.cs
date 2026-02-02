using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class GameRepository(DuelNexusDbContext context) : Repository<Game>(context), IGameRepository
{
    public async Task<Game> CreateGameFromRoomAsync(GameRoom gameRoom)
    {
        var game = new Game
        {
            StartedAt = DateTime.UtcNow,
            Room = gameRoom,
        };

        await AddAsync(game);
        return game;
    }
}
