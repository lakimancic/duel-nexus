using Backend.Data.Context;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class CardMovementRepository(DuelNexusDbContext context) : Repository<CardMovementAction>(context), ICardMovementRepository
{
    public Task<int> CountDrawsInTurnByPlayerAsync(Guid turnId, Guid playerGameId)
    {
        return _dbSet
            .Where(action =>
                action.TurnId == turnId &&
                action.MovementType == CardMovementType.Draw &&
                action.Card.PlayerGameId == playerGameId)
            .CountAsync();
    }

    public Task DrawActionAsync(GameCard gameCard, Turn turn)
    {
        var action = new CardMovementAction
        {
            MovementType = CardMovementType.Draw,
            Card = gameCard,
            Turn = turn,
        };

        return AddAsync(action);
    }

    public Task<CardMovementAction?> GetByIdWithIncludesAsync(Guid id)
    {
        return _dbSet
            .Where(a => a.Id == id)
            .Include(a => a.Turn)
            .Include(a => a.Card).ThenInclude(gc => gc.Card)
            .FirstOrDefaultAsync();
    }
}
