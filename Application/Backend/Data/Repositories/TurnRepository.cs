using Backend.Data.Context;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class TurnRepository(DuelNexusDbContext context) : Repository<Turn>(context), ITurnRepository
{
    public async Task InitializeTurnsForGameAsync(Game game)
    {
        var firstTurn = new Turn
        {
            Game = game,
            TurnNumber = 1,
            Phase = TurnPhase.Draw,
        };
        await AddAsync(firstTurn);
    }
}
