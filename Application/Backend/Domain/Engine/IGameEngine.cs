namespace Backend.Domain.Engine;

using Backend.Data.Models;
using Backend.Domain.Commands;

public interface IGameEngine
{
    Task InitializeGameAsync(Game game, CancellationToken cancellationToken = default);
    Task<TResult> ExecuteCommandAsync<TResult>(Guid gameId, Guid userId, IGameCommand<TResult> command, CancellationToken cancellationToken = default);
    Task<GameStateSnapshot> GetGameStateAsync(Guid gameId, Guid viewerUserId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsInGameAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default);
}
