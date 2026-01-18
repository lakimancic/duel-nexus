using Backend.Infrastructure.Persistence.IRepositories;
using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Backend.Infrastructure.Persistence.Repositories;

public class UserRepository(DuelNexusDbContext context, IMapper mapper) : Repository<User, UserEntity>(context, mapper), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users
            .Where(u => u.Email == email)
            .Select(u => _mapper.Map<User>(u))
            .FirstOrDefaultAsync();

    public async Task<bool> ExistsAsync(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);
}
