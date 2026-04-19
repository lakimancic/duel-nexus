namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain;

public sealed class RestoreCardsEffectStrategy : IEffectStrategy
{
    public async Task ApplyAsync(Effect effect, EffectExecutionContext context, CancellationToken cancellationToken = default)
    {
        var cardsToRestore = Math.Max(1, effect.Affects ?? 1);
        var currentHandCount = await context.UnitOfWork.GameCards.CountByPlayerAndZoneAsync(context.ActivatingPlayer.Id, CardZone.Hand);
        var handSlotsLeft = Math.Max(0, GameConstants.MaxHandSize - currentHandCount);
        if (handSlotsLeft == 0)
            return;

        cardsToRestore = Math.Min(cardsToRestore, handSlotsLeft);
        var allCards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);
        var ownGraveCards = allCards
            .Where(card =>
                card.PlayerGameId == context.ActivatingPlayer.Id &&
                card.Zone == CardZone.Grave)
            .OrderByDescending(card => card.DeckOrder ?? -1)
            .Take(cardsToRestore)
            .ToList();

        foreach (var card in ownGraveCards)
        {
            cancellationToken.ThrowIfCancellationRequested();
            card.Zone = CardZone.Hand;
            card.FieldIndex = null;
            card.IsFaceDown = false;
            card.DefensePosition = false;
            card.DeckOrder = null;
            context.UnitOfWork.GameCards.Update(card);

            context.AppliedTargetCardIds.Add(card.Id);
            context.AppliedTargetPlayerIds.Add(card.PlayerGameId);
        }
    }
}
