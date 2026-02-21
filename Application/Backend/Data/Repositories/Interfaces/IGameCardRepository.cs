using Backend.Data.Models;
using Backend.Data.Enums;

namespace Backend.Data.Repositories.Interfaces;

public interface IGameCardRepository : IRepository<GameCard>
{
    Task CreateGameCardsAsync(PlayerGame pg, List<DeckCard> deckCards);
    Task<int> CountByPlayerAndZoneAsync(Guid playerGameId, CardZone zone);
    Task<GameCard?> GetTopDeckCardByPlayerWithCardAsync(Guid playerGameId);
    Task<List<GameCard>> GetTopDeckCardsByPlayerAsync(Guid playerGameId, int take);
    Task<GameCard?> GetByWithCardById(Guid id);
    Task<List<GameCard>> GetByGameIdWithCardAsync(Guid gameId);
}
