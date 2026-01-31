using Backend.Data.Context;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICardRepository Cards { get; }
    IDeckRepository Decks { get; }
    IDeckCardRepository DeckCardRepository { get; }
    IGameRepository Games { get; }
    IPlayerGameRepository PlayerGames { get; }
    IGameRoomRepository GameRooms { get; }
    IGameRoomPlayerRepository GameRoomPlayers { get; }
    ITurnRepository Turns { get; }
    IEffectRepository Effects { get; }
    IPlayerCardRepository PlayerCards { get; }



    public DuelNexusDbContext Context { get; }

    Task CompleteAsync();
}
