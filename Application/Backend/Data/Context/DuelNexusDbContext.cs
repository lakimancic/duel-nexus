using Microsoft.EntityFrameworkCore;
using Backend.Data.Models;

namespace Backend.Data.Context;

public class DuelNexusDbContext(DbContextOptions<DuelNexusDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
