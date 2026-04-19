namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed class DestroyCardsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var nextGraveOrderByPlayer = new Dictionary<Guid, int>();
        var protectedCardIds = await ProtectionEffectHelper.GetProtectedCardIdsAsync(
            context.UnitOfWork,
            context.Game.Id,
            context.Turn,
            cancellationToken);

        var hasRequestedTargets = (context.RequestedTargetCardIds?.Count ?? 0) > 0;

        // Trap responses default to attacker when target wasn't explicitly chosen.
        if (context.IsTrapResponse &&
            !hasRequestedTargets &&
            context.AttackAttackerCard is not null &&
            context.AttackAttackerCard.Zone == CardZone.Field &&
            !protectedCardIds.Contains(context.AttackAttackerCard.Id))
        {
            await SendCardToGraveyard(context.AttackAttackerCard, context, nextGraveOrderByPlayer);
            context.AppliedTargetCardIds.Add(context.AttackAttackerCard.Id);
            context.AppliedTargetPlayerIds.Add(context.AttackAttackerCard.PlayerGameId);
            return;
        }

        var cardsToDestroy = Math.Max(1, effect.Affects ?? 1);
        var allCards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);
        var validEnemyFieldCards = allCards
            .Where(card =>
                card.PlayerGameId != context.ActivatingPlayer.Id &&
                card.Zone == CardZone.Field &&
                card.FieldIndex is not null)
            .ToDictionary(card => card.Id, card => card);

        var requestedTargets = (context.RequestedTargetCardIds ?? [])
            .Where(targetId => validEnemyFieldCards.ContainsKey(targetId))
            .Select(targetId => validEnemyFieldCards[targetId])
            .DistinctBy(card => card.Id)
            .ToList();

        var targets = (requestedTargets.Count > 0
            ? requestedTargets
            : validEnemyFieldCards.Values
                .OrderBy(card => card.FieldIndex)
                .ToList())
            .Where(card => !protectedCardIds.Contains(card.Id))
            .Take(cardsToDestroy)
            .ToList();

        foreach (var target in targets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await SendCardToGraveyard(target, context, nextGraveOrderByPlayer);
            context.AppliedTargetCardIds.Add(target.Id);
            context.AppliedTargetPlayerIds.Add(target.PlayerGameId);
        }
    }

    private static async Task SendCardToGraveyard(
        GameCard card,
        EffectExecutionContext context,
        Dictionary<Guid, int> nextGraveOrderByPlayer)
    {
        if (!nextGraveOrderByPlayer.TryGetValue(card.PlayerGameId, out var nextOrder))
        {
            nextOrder = await context.UnitOfWork.GameCards.GetNextGraveOrderAsync(card.PlayerGameId);
        }

        card.Zone = CardZone.Grave;
        card.FieldIndex = null;
        card.IsFaceDown = false;
        card.DefensePosition = false;
        card.DeckOrder = nextOrder;
        context.UnitOfWork.GameCards.Update(card);
        nextGraveOrderByPlayer[card.PlayerGameId] = nextOrder + 1;
    }
}
