using Backend.Domain.Entities;

namespace Backend.Application.Services;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task AddAsync(User user);
}