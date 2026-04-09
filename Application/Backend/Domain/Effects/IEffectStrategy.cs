namespace Backend.Domain.Effects;

using Backend.Data.Models;

public interface IEffectStrategy
{
    Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default);
}
