namespace Backend.Domain.Effects;

using Backend.Data.Models;

public sealed class RestoreCardsEffectStrategy : IEffectStrategy
{
    public Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        // Simplified implementation keeps this as a no-op for now.
        return Task.CompletedTask;
    }
}
