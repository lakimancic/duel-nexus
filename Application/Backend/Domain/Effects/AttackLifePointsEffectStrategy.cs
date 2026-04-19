namespace Backend.Domain.Effects;

using Backend.Data.Models;

public sealed class AttackLifePointsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var damage = Math.Max(0, effect.Points ?? 0);
        if (damage <= 0)
            return;

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        var validTargets = players
            .Where(player => player.Id != context.ActivatingPlayer.Id && player.LifePoints > 0)
            .ToDictionary(player => player.Id, player => player);

        var requestedPlayers = (context.RequestedTargetPlayerIds ?? [])
            .Where(playerId => validTargets.ContainsKey(playerId))
            .Select(playerId => validTargets[playerId])
            .DistinctBy(player => player.Id)
            .ToList();

        var targets = requestedPlayers.Count > 0
            ? requestedPlayers
            : validTargets.Values.Take(1).ToList();

        foreach (var target in targets)
        {
            cancellationToken.ThrowIfCancellationRequested();

            target.LifePoints = Math.Max(0, target.LifePoints - damage);
            if (target.LifePoints == 0)
                target.TurnEnded = true;

            context.UnitOfWork.PlayerGames.Update(target);
            context.AppliedTargetPlayerIds.Add(target.Id);
        }
    }
}
