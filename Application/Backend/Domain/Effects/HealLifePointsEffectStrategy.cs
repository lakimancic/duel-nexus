namespace Backend.Domain.Effects;

using Backend.Data.Models;

public sealed class HealLifePointsEffectStrategy : IEffectStrategy
{
    public Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var heal = Math.Max(0, effect.Points ?? 0);
        if (heal <= 0)
            return Task.CompletedTask;

        context.ActivatingPlayer.LifePoints += heal;
        context.UnitOfWork.PlayerGames.Update(context.ActivatingPlayer);
        context.AppliedTargetPlayerIds.Add(context.ActivatingPlayer.Id);
        return Task.CompletedTask;
    }
}
