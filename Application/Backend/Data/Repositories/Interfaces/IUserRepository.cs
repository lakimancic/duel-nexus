using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<List<User>> GetBySetIds(List<Guid> onlineUserIds);
    Task<List<User>> GetTopByEloAsync(int limit);
}
