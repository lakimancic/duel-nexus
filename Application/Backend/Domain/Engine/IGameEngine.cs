namespace Backend.Domain.Engine;

using Backend.Data.Models;
using Backend.Domain.Commands;

public interface IGameEngine
{
    Task InitializeGameAsync(Game game, CancellationToken cancellationToken = default);
    Task<TResult> ExecuteCommandAsync<TCommand, TResult>(Guid gameId, Guid userId, TCommand command, CancellationToken cancellationToken = default)
        where TCommand : IGameCommand<TResult>;
    Task<GameStateSnapshot> GetGameStateAsync(Guid gameId, Guid viewerUserId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsInGameAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default);
}
