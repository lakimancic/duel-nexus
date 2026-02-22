namespace Backend.Domain.Engine;

using System.Collections.Concurrent;
using System.Threading;

public sealed class GameCommandLock : IGameCommandLock
{
    private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

    public async Task<TResult> ExecuteAsync<TResult>(Guid gameId, Func<Task<TResult>> action)
    {
        var semaphore = _locks.GetOrAdd(gameId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            semaphore.Release();
        }
    }
}
