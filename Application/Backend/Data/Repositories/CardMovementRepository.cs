using Backend.Data.Context;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class CardMovementRepository(DuelNexusDbContext context) : Repository<CardMovementAction>(context), ICardMovementRepository
{
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
}
