using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameCardRepository : IRepository<GameCard>
{
    Task CreateGameCardsAsync(PlayerGame pg, List<DeckCard> deckCards);
    Task<GameCard?> GetByWithCardById(Guid id);
    Task<List<GameCard>> GetByGameIdWithCardAsync(Guid gameId);
}
