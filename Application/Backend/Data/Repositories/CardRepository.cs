using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class CardRepository(DuelNexusDbContext context) : Repository<Card>(context), ICardRepository
{
    public async Task<Card?> GetCardWithEffectAsync(Guid id)
    {
        var card = await _context.Set<Card>()
            .Where(c => c.Id == id)
            .Include(c => c.Effect)
            .FirstOrDefaultAsync();
        return card;
    }
}
