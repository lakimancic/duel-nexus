using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class DeckRepository(DuelNexusDbContext context) : Repository<Deck>(context), IDeckRepository
{
    public Task<List<Deck>> GetByUserId(Guid userId)
    {
        return _context.Set<Deck>()
            .Where(d => d.UserId == userId)
            .Include(d => d.User)
            .ToListAsync();
    }

    public async Task<Deck?> GetDeckWithUser(Guid id)
    {
        var deck = await _context.Set<Deck>()
            .Where(d => d.Id == id)
            .Include(d => d.User)
            .FirstOrDefaultAsync();
        return deck;
    }
}
