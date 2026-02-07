using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class DeckCardRepository(DuelNexusDbContext context) : Repository<DeckCard>(context), IDeckCardRepository
{
    public async Task AddCardsInDeckAsync(Guid deckId, List<DeckCard> cardsWithQuantity)
    {
        var cardIds = cardsWithQuantity.Select(c => c.CardId).ToList();

        var existingCards = await _dbSet
            .Where(dc => dc.DeckId == deckId && cardIds.Contains(dc.CardId))
            .ToDictionaryAsync(dc => dc.CardId, dc => dc);

        foreach (var card in cardsWithQuantity)
        {
            if (existingCards.TryGetValue(card.CardId, out var existing))
            {
                existing.Quantity += card.Quantity;
            }
            else
            {
                card.DeckId = deckId;
                await AddAsync(card);
            }
        }
    }

    public async Task DeleteManyCardAsync(Guid id, List<Guid> cardIds)
    {
        var deletedCount = await _dbSet
            .Where(c => c.DeckId == id && cardIds.Contains(c.CardId))
            .ExecuteDeleteAsync();

        if (deletedCount == 0)
            throw new ObjectNotFoundException("No matching cards found in the deck.");
    }

    public Task<List<DeckCard>> GetByDeckId(Guid deckId)
    {
        return _dbSet
            .Where(dc => dc.DeckId == deckId)
            .Include(dc => dc.Card)
            .ToListAsync();
    }
}
