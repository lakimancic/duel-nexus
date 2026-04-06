using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IAttackRepository : IRepository<AttackAction>
{
	Task<AttackAction?> GetByIdWithIncludesAsync(Guid id);
    Task<bool> HasAttackForTurnAndAttackerAsync(Guid turnId, Guid attackerCardId);
    Task<HashSet<Guid>> GetAttackerCardIdsByTurnAsync(Guid turnId);
}
