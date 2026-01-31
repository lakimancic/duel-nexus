using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class GameRepository(DuelNexusDbContext context) : Repository<Game>(context), IGameRepository
{
    public async Task<Game> CreateFromGameRoomAsync(GameRoom gameRoom)
    {
        if (gameRoom == null) throw new ArgumentNullException(nameof(gameRoom));

        var game = new Game
        {
            StartedAt = DateTime.UtcNow,
            Room = gameRoom,
            Players = [],
        };

        foreach (var grp in gameRoom.Players)
        {
            var pg = new PlayerGame
            {
                User = grp.User,
                UserId = grp.UserId,
                Game = game,
                LifePoints = 8000
            };
            game.Players.Add(pg);
        }

        await AddAsync(game);
        return game;
    }
}
