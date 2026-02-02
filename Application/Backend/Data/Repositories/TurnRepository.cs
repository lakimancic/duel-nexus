using Backend.Data.Context;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class TurnRepository(DuelNexusDbContext context) : Repository<Turn>(context), ITurnRepository
{
    public async Task<Turn?> GetCurrentTurnAsync(Guid gameId)
    {
        return await _context.Turns
            .Where(t => t.GameId == gameId)
            .OrderByDescending(t => t.TurnNumber)
            .FirstOrDefaultAsync();
    }

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

    public Task NextTurnAsync(Turn turn)
    {
        var nextTurn = new Turn
        {
            GameId = turn.GameId,
            TurnNumber = turn.TurnNumber + 1,
            Phase = TurnPhase.Draw,
        };
        return AddAsync(nextTurn);
    }
}
