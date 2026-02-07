using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class PlayerCardRepository(DuelNexusDbContext context) : Repository<PlayerCard>(context), IPlayerCardRepository
{
    public Task<List<PlayerCard>> GetCardsByUserId(Guid userId)
    {
        var cards = _dbSet
            .Where(pc => pc.UserId == userId)
            .Include(pc => pc.Card)
            .ToListAsync();
        return cards;
    }

    public async Task<PlayerCard?> GetPlayerCard(Guid userId, Guid cardId)
    {
        var card = await _dbSet
            .Where(pc => pc.UserId == userId && pc.CardId == cardId)
            .Include(pc => pc.Card)
            .FirstOrDefaultAsync();
        return card;
    }
}
