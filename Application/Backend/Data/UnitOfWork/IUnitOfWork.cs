using Backend.Data.Context;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.UnitOfWork;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ICardRepository Cards { get; }
    ICardMovementRepository CardMovements { get; }
    IMessageRepository Messages { get; }
    IDeckRepository Decks { get; }
    IDeckCardRepository DeckCards { get; }
    IGameRepository Games { get; }
    IGameCardRepository GameCards { get; }
    IPlayerGameRepository PlayerGames { get; }
    IGameRoomRepository GameRooms { get; }
    IGameRoomPlayerRepository GameRoomPlayers { get; }
    ITurnRepository Turns { get; }
    IEffectRepository Effects { get; }
    IPlayerCardRepository PlayerCards { get; }

    public DuelNexusDbContext Context { get; }

    Task CompleteAsync();
}
