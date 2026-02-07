using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class EffectActivationRepository(DuelNexusDbContext context) : Repository<EffectActivation>(context), IEffectActivationRepository
{
    public async Task<EffectActivation> ActivateEffectAsync(Turn turn, Effect effect, GameCard gameCard)
    {
        var activation = new EffectActivation
        {
            Turn=turn,
            Effect=effect,
            SourceCard=gameCard
        };
        await AddAsync(activation);
        return activation;
    }

    public Task<EffectActivation?> GetByIdWithIncludesAsync(Guid id)
    {
        return _dbSet
            .Where(a => a.Id == id)
            .Include(a => a.Turn)
            .Include(a => a.SourceCard).ThenInclude(gc => gc.Card)
            .FirstOrDefaultAsync();
    }
}
