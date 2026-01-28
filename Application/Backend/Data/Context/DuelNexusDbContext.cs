using Microsoft.EntityFrameworkCore;
using Backend.Data.Models;
using Backend.Data.Enums;

namespace Backend.Data.Context;

public class DuelNexusDbContext(DbContextOptions<DuelNexusDbContext> options) : DbContext(options)
{
    public DbSet<AttackAction> AttackActions => Set<AttackAction>();
    public DbSet<Card> Cards => Set<Card>();
    public DbSet<CardMovementAction> CardMovementActions => Set<CardMovementAction>();
    public DbSet<Deck> Decks => Set<Deck>();
    public DbSet<DeckCard> DeckCards => Set<DeckCard>();
    public DbSet<Effect> Effects => Set<Effect>();
    public DbSet<EffectActivation> EffectActivations => Set<EffectActivation>();
    public DbSet<EffectResult> EffectResults => Set<EffectResult>();
    public DbSet<EffectTarget> EffectTargets => Set<EffectTarget>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameCard> GameCards => Set<GameCard>();
    public DbSet<GameRoom> GameRooms => Set<GameRoom>();
    public DbSet<GameRoomPlayer> GameRoomPlayers => Set<GameRoomPlayer>();
    public DbSet<PlayerGame> PlayerGames => Set<PlayerGame>();
    public DbSet<PlaceCardAction> PlaceCardActions => Set<PlaceCardAction>();
    public DbSet<Turn> Turns => Set<Turn>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Deck> Decks => Set<Deck>();
    public DbSet<Card> Cards => Set<Card>();


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
