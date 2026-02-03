using Backend.Data.Context;
using Backend.Data.Repositories;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.UnitOfWork;

public class UnitOfWork(DuelNexusDbContext context) : IUnitOfWork
{
    private readonly DuelNexusDbContext _context = context;
    public IAttackRepository Attacks { get; private set; } = new AttackRepository(context);
    public IUserRepository Users { get; private set; } = new UserRepository(context);
    public ICardRepository Cards { get; private set; } = new CardRepository(context);
    public ICardMovementRepository CardMovements { get; private set; } = new CardMovementRepository(context);
    public IMessageRepository Messages { get; private set; } = new MessageRepository(context);
    public IDeckRepository Decks { get; private set; } = new DeckRepository(context);
    public IDeckCardRepository DeckCards { get; private set; } = new DeckCardRepository(context);
    public IGameRepository Games { get; private set; } = new GameRepository(context);
    public IGameCardRepository GameCards { get; private set; } = new GameCardRepository(context);
    public IPlayerGameRepository PlayerGames { get; private set; } = new PlayerGameRepository(context);
    public IGameRoomRepository GameRooms { get; private set; } = new GameRoomRepository(context);
    public IGameRoomPlayerRepository GameRoomPlayers { get; private set; } = new GameRoomPlayerRepository(context);
    public ITurnRepository Turns { get; private set; } = new TurnRepository(context);
    public IEffectRepository Effects { get; private set; } = new EffectRepository(context);
    public IEffectActivationRepository EffectActivations { get; private set; } = new EffectActivationRepository(context);
    public IEffectTargetRepository EffectTargets { get; private set; } = new EffectTargetRepository(context);
    public IPlayerCardRepository PlayerCards { get; private set; } = new PlayerCardRepository(context);
    public IPlaceCardRepository PlaceCards { get; private set; } = new PlaceCardRepository(context);


    public DuelNexusDbContext Context => _context;

    public async Task CompleteAsync() => await _context.SaveChangesAsync();
}
