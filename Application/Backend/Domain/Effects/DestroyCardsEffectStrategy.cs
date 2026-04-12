namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed class DestroyCardsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var nextGraveOrderByPlayer = new Dictionary<Guid, int>();

        // Trap responses can target the attacking monster directly.
        if (context.IsTrapResponse && context.AttackAttackerCard is not null && context.AttackAttackerCard.Zone == CardZone.Field)
        {
            await SendCardToGraveyard(context.AttackAttackerCard, context, nextGraveOrderByPlayer);
            return;
        }

        var cardsToDestroy = Math.Max(1, effect.Affects ?? 1);
        var allCards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);

        var targets = allCards
            .Where(card =>
                card.PlayerGameId != context.ActivatingPlayer.Id &&
                card.Zone == CardZone.Field &&
                card.FieldIndex is not null)
            .OrderBy(card => card.FieldIndex)
            .Take(cardsToDestroy)
            .ToList();

        foreach (var target in targets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await SendCardToGraveyard(target, context, nextGraveOrderByPlayer);
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
