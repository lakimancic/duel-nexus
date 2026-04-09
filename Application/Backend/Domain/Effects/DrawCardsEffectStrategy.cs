namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;

public sealed class DrawCardsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var cardsToDraw = Math.Max(1, effect.Affects ?? 1);
        for (var i = 0; i < cardsToDraw; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var topDeckCard = await context.UnitOfWork.GameCards.GetTopDeckCardByPlayerWithCardAsync(context.ActivatingPlayer.Id);
            if (topDeckCard is null)
                break;

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
