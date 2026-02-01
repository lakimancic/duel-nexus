using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Backend.Utils.Rand;

namespace Backend.Data.Repositories;

public class GameRepository(DuelNexusDbContext context) : Repository<Game>(context), IGameRepository
{
    public async Task<Game> CreateFromGameRoomAsync(GameRoom gameRoom)
    {
        var game = new Game
        {
            StartedAt = DateTime.UtcNow,
            Room = gameRoom,
            Players = [],
        };

        var playerIndices = RandomExtensions.GenerateUniqueRandomIndices(gameRoom.Players.Count);
        int indexCounter = 0;

        foreach (var grp in gameRoom.Players)
        {
            if (!grp.IsReady)
                throw new InvalidOperationException("All players must be ready to start the game.");

            var pg = new PlayerGame
            {
                User = grp.User,
                DeckTemplate = grp.Deck!,
                Game = game,
                LifePoints = 8000,
                Index = playerIndices[indexCounter++],
            };
            game.Players.Add(pg);
        }

        await AddAsync(game);
        return game;
    }
}
