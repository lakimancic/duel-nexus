using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameCardRepository : IRepository<GameCard>
{
    Task CreateGameCardsAsync(PlayerGame pg, List<DeckCard> deckCards);
    Task<List<GameCard>> GetByPlayerGameIdAsync(Guid id);
}
