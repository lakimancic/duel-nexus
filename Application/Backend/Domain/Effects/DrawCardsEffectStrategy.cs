namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain;

public sealed class DrawCardsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var cardsToDraw = Math.Max(1, effect.Affects ?? 1);
        var handCount = await context.UnitOfWork.GameCards.CountByPlayerAndZoneAsync(context.ActivatingPlayer.Id, CardZone.Hand);
        var handSlotsLeft = Math.Max(0, GameConstants.MaxHandSize - handCount);
        if (handSlotsLeft == 0)
            return;

        cardsToDraw = Math.Min(cardsToDraw, handSlotsLeft);
        var topDeckCards = await context.UnitOfWork.GameCards.GetTopDeckCardsByPlayerAsync(
            context.ActivatingPlayer.Id,
            cardsToDraw);

        foreach (var topDeckCard in topDeckCards)
        {
            cancellationToken.ThrowIfCancellationRequested();

            topDeckCard.Zone = CardZone.Hand;
            topDeckCard.DeckOrder = null;
            context.UnitOfWork.GameCards.Update(topDeckCard);

            await context.UnitOfWork.CardMovements.AddAsync(new CardMovementAction
            {
                TurnId = context.Turn.Id,
                GameCardId = topDeckCard.Id,
                FromZone = CardZone.Deck,
                ToZone = CardZone.Hand,
                MovementType = CardMovementType.Draw,
                ExecutedAt = DateTime.UtcNow,
            });
        }
    }
}
