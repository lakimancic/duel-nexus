namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed class ProtectCardsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var cardsToProtect = Math.Max(1, effect.Affects ?? 1);
        var allCards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);

        var validOwnFieldCards = allCards
            .Where(card =>
                card.PlayerGameId == context.ActivatingPlayer.Id &&
                card.Zone == CardZone.Field &&
                card.FieldIndex is not null)
            .ToDictionary(card => card.Id, card => card);

        var requestedTargets = (context.RequestedTargetCardIds ?? [])
            .Where(targetId => validOwnFieldCards.ContainsKey(targetId))
            .Select(targetId => validOwnFieldCards[targetId])
            .DistinctBy(card => card.Id)
            .ToList();

        var targets = (requestedTargets.Count > 0
            ? requestedTargets
            : validOwnFieldCards.Values.OrderBy(card => card.FieldIndex).ToList())
            .Take(cardsToProtect);

        foreach (var target in targets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            context.AppliedTargetCardIds.Add(target.Id);
            context.AppliedTargetPlayerIds.Add(target.PlayerGameId);
        }
    }
}
