using Backend.Data.Context;
using Backend.Data.Repositories;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.UnitOfWork;

public class UnitOfWork(DuelNexusDbContext context) : IUnitOfWork
{
    private readonly DuelNexusDbContext _context = context;
    public IUserRepository Users { get; private set; } = new UserRepository(context);
    public ICardRepository Cards { get; private set; } = new CardRepository(context);
    public IMessageRepository Messages { get; private set; } = new MessageRepository(context);
    public IDeckRepository Decks { get; private set; } = new DeckRepository(context);
    public IDeckCardRepository DeckCardRepository { get; private set; } = new DeckCardRepository(context);
    public IGameRepository Games { get; private set; } = new GameRepository(context);
    public IPlayerGameRepository PlayerGames { get; private set; } = new PlayerGameRepository(context);
    public IGameRoomRepository GameRooms { get; private set; } = new GameRoomRepository(context);
    public IGameRoomPlayerRepository GameRoomPlayers { get; private set; } = new GameRoomPlayerRepository(context);
    public ITurnRepository Turns { get; private set; } = new TurnRepository(context);
    public IEffectRepository Effects { get; private set; } = new EffectRepository(context);
    public IPlayerCardRepository PlayerCards { get; private set; } = new PlayerCardRepository(context);


    private bool _disposed = false;

    public DuelNexusDbContext Context => _context;

    public async Task CompleteAsync() => await _context.SaveChangesAsync();
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
