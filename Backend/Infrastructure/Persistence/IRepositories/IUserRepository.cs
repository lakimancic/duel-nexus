using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence.Entities;

namespace Backend.Infrastructure.Persistence.IRepositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
}