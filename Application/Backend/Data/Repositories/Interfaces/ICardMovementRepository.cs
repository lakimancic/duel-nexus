using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface ICardMovementRepository : IRepository<CardMovementAction>
{
    Task DrawActionAsync(GameCard gameCard, Turn turn);
}
