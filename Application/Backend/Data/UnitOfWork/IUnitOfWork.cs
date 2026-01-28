using Backend.Data.Context;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICardRepository Cards { get; }
    IDeckRepository Decks { get; }
    IDeckCardRepository DeckCardRepository { get; }
    public DuelNexusDbContext Context { get; }

    Task CompleteAsync();
}
