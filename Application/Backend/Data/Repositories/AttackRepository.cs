using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class AttackRepository(DuelNexusDbContext context) : Repository<AttackAction>(context), IAttackRepository
{
	public Task<AttackAction?> GetByIdWithIncludesAsync(Guid id)
	{
		return _dbSet
			.Where(a => a.Id == id)
			.Include(a => a.Turn)
			.Include(a => a.Attacker).ThenInclude(gc => gc.Card)
			.Include(a => a.Defender!).ThenInclude(gc => gc.Card)
			.Include(a => a.DefenderPlayer!).ThenInclude(pg => pg.User)
			.FirstOrDefaultAsync();
	}

    public Task<bool> HasAttackForTurnAndAttackerAsync(Guid turnId, Guid attackerCardId)
    {
        return _dbSet.AnyAsync(action =>
            action.TurnId == turnId &&
            action.AttackerCardId == attackerCardId);
    }

    public async Task<HashSet<Guid>> GetAttackerCardIdsByTurnAsync(Guid turnId)
    {
        return await _dbSet
            .Where(action => action.TurnId == turnId)
            .Select(action => action.AttackerCardId)
            .ToHashSetAsync();
    }
}
