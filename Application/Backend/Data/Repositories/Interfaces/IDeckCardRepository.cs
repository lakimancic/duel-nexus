using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IDeckCardRepository : IRepository<DeckCard>
{
    Task AddCardsInDeckAsync(Guid deckId, IEnumerable<(Card card, int quantity)> cardsWithQuantity);
}