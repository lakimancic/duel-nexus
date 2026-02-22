namespace Backend.Domain.Engine;

public interface IGameCommandLock
{
    Task<TResult> ExecuteAsync<TResult>(Guid gameId, Func<Task<TResult>> action);
}
