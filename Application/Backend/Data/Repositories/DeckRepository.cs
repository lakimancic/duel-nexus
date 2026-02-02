using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class DeckRepository(DuelNexusDbContext context) : Repository<Deck>(context), IDeckRepository
{
    public Task<List<Deck>> GetByUserId(Guid userId)
    {
        return _context.Decks.Where(d => d.UserId == userId).ToListAsync();
    }

}
