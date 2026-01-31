using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class DeckCardRepository(DuelNexusDbContext context) : Repository<DeckCard>(context), IDeckCardRepository
{
    public async Task AddCardsInDeckAsync(Guid deckId, List<DeckCard> cardsWithQuantity)
    {
        var cardIds = cardsWithQuantity.Select(c => c.CardId).ToList();

        var existingCards = await _context.Set<DeckCard>()
            .Where(dc => dc.DeckId == deckId && cardIds.Contains(dc.CardId))
            .ToListAsync();

        foreach (var card in cardsWithQuantity)
        {
            var existing = existingCards.FirstOrDefault(c => c.CardId == card.CardId);
            if (existing != null)
            {
                existing.Quantity += card.Quantity;
            }
            else
            {
                card.DeckId = deckId;
                _context.Set<DeckCard>().Add(card);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteManyCardAsync(Guid id, List<Guid> cardIds)
    {
        var cards = await _context.Set<DeckCard>()
            .Where(c => c.DeckId == id && cardIds.Contains(c.CardId))
            .ToListAsync();

            if (cards.Count == 0)
                throw new Exception("No matching cards found in the deck.");

        _context.Set<DeckCard>().RemoveRange(cards);
    }

    public Task<List<DeckCard>> GetByDeckId(Guid deckId)
    {
        return _context.Set<DeckCard>()
            .Where(dc => dc.DeckId == deckId)
            .ToListAsync();
    }
}
