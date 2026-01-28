using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class DeckCardRepository(DuelNexusDbContext context) : Repository<DeckCard>(context), IDeckCardRepository
{
    public async Task AddCardsInDeckAsync(Guid deckId, IEnumerable<(Card card, int quantity)> cardsWithQuantity)
    {
        var deckExists = await _context.Set<Deck>().AnyAsync(d => d.Id == deckId);
        
        if (!deckExists)
            return;

        foreach (var (card, quantity) in cardsWithQuantity)
        {
            var deckCard = new DeckCard
            {
                Id = Guid.NewGuid(),
                DeckId = deckId,
                CardId = card.Id,
                Quantity = quantity
            };
            
            await _context.Set<DeckCard>().AddAsync(deckCard);
        }

        await _context.SaveChangesAsync();
    }
}