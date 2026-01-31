using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IPlayerCardRepository : IRepository<PlayerCard>
{
    Task<List<PlayerCard>> GetCardsByDeckId(Guid deckId);

}
