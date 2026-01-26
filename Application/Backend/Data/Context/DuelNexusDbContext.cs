using Microsoft.EntityFrameworkCore;
using Backend.Data.Models;
using Backend.Data.Enums;

namespace Backend.Data.Context;

public class DuelNexusDbContext(DbContextOptions<DuelNexusDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>()
            .HasDiscriminator<CardType>("Type")
            .HasValue<MonsterCard>(CardType.Monster)
            .HasValue<SpellCard>(CardType.Spell)
            .HasValue<TrapCard>(CardType.Trap);

        base.OnModelCreating(modelBuilder);
    }
}
