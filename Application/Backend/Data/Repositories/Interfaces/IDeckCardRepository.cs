using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IDeckCardRepository : IRepository<DeckCard>
{
    Task AddCardsInDeckAsync(Guid deckId, List<DeckCard> cardsWithQuantity);
    Task DeleteManyCardAsync(Guid deckId, List<Guid> cardIds);

}