using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class UserRepository(DuelNexusDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();

    public async Task<bool> ExistsAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);
}
