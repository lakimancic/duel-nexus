using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IPlayerCardRepository : IRepository<PlayerCard>
{
    Task<List<PlayerCard>> GetCardsByUserId(Guid userId);
    Task<PlayerCard?> GetPlayerCard(Guid userId, Guid cardId);
}
