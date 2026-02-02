using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IDeckRepository : IRepository<Deck>
{
    Task<List<Deck>> GetByUserId(Guid userId);
}
