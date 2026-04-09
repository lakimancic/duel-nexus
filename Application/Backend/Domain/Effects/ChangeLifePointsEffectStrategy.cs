namespace Backend.Domain.Effects;

using Backend.Data.Models;

public sealed class ChangeLifePointsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        var points = effect.Points ?? 0;
        if (points == 0)
            return;

        if (context.IsTrapResponse && context.AttackAttackerPlayer is not null && context.AttackDefenderPlayer is not null)
        {
            if (points > 0)
            {
                context.AttackDefenderPlayer.LifePoints += points;
                context.UnitOfWork.PlayerGames.Update(context.AttackDefenderPlayer);
                return;
            }

            var damage = Math.Abs(points);
            context.AttackAttackerPlayer.LifePoints = Math.Max(0, context.AttackAttackerPlayer.LifePoints - damage);
            if (context.AttackAttackerPlayer.LifePoints == 0)
                context.AttackAttackerPlayer.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(context.AttackAttackerPlayer);
            return;
        }

        if (points > 0)
        {
            context.ActivatingPlayer.LifePoints += points;
            context.UnitOfWork.PlayerGames.Update(context.ActivatingPlayer);
            return;
        }

        var damageToOpponents = Math.Abs(points);
        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        foreach (var player in players.Where(player => player.Id != context.ActivatingPlayer.Id && player.LifePoints > 0))
        {
            cancellationToken.ThrowIfCancellationRequested();
            player.LifePoints = Math.Max(0, player.LifePoints - damageToOpponents);
            if (player.LifePoints == 0)
                player.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(player);
        }
    }
}
