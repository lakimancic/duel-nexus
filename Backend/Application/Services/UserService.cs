using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence.IRepositories;

namespace Backend.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _userRepository.ExistsAsync(email);
    }

    public async Task AddAsync(User user)
    {
        await _userRepository.AddAsync(user);
    }
}