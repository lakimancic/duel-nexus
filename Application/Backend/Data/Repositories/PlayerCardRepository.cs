using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class PlayerCardRepository(DuelNexusDbContext context) : Repository<PlayerCard>(context), IPlayerCardRepository
{
    public async Task<List<PlayerCard>> GetCardsByDeckId(Guid deckId)
    {
        return await _context.PlayerCards.Where(p => p.DeckId == deckId).ToListAsync();
    }
}
