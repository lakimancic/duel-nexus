using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface ICardRepository : IRepository<Card>
{
    Task<Card?> GetCardWithEffectAsync(Guid id);
}
