namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class CardEffectExecutor
{
    private readonly EffectStrategyFactory _strategyFactory = new();

    public async Task<GameEffectActivationSummary?> TryActivateSpellOnPlacementAsync(
        GameCommandContext commandContext,
        GameCard placedCard,
        CancellationToken cancellationToken = default)
    {
        if (placedCard.Card is not SpellCard || placedCard.IsFaceDown)
            return null;

        var effect = placedCard.Card.Effect;
        if (effect is null)
        {
            await SendCardToGraveyard(placedCard, commandContext.UnitOfWork);
            return null;
        }

        var executionContext = new EffectExecutionContext(
            UnitOfWork: commandContext.UnitOfWork,
            Game: commandContext.Game,
            Turn: commandContext.CurrentTurn,
            ActivatingPlayer: commandContext.Actor,
            SourceCard: placedCard,
            IsTrapResponse: false,
            AttackAttackerCard: null,
            AttackAttackerPlayer: null,
            AttackDefenderPlayer: null);

        var strategy = _strategyFactory.GetStrategy(effect.Type);
        await strategy.ApplyAsync(effect, executionContext, cancellationToken);
        await SendCardToGraveyard(placedCard, commandContext.UnitOfWork);

        var activation = await commandContext.UnitOfWork.EffectActivations.ActivateEffectAsync(
            commandContext.CurrentTurn,
            effect,
            placedCard);
        activation.Resolved = true;
        commandContext.UnitOfWork.EffectActivations.Update(activation);

        return new GameEffectActivationSummary(
            SourceCardId: placedCard.Id,
            ActivatedByPlayerGameId: commandContext.Actor.Id,
            SourceCardName: placedCard.Card.Name,
            EffectType: effect.Type,
            IsTrap: false);
    }

    public async Task<GameEffectActivationSummary?> TryActivateTrapDuringAttackAsync(
        GameCommandContext commandContext,
        PlayerGame defendingPlayer,
        PlayerGame attackingPlayer,
        GameCard attackerCard,
        Guid? trapCardId,
        CancellationToken cancellationToken = default)
    {
        if (!trapCardId.HasValue)
            return null;

        var trapCard = await commandContext.UnitOfWork.GameCards.GetByWithCardById(trapCardId.Value)
            ?? throw new ObjectNotFoundException("Trap card not found.");

        if (trapCard.PlayerGameId != defendingPlayer.Id)
            throw new BadRequestException("You can activate only your own trap cards.");

        if (trapCard.Zone != CardZone.Field || trapCard.FieldIndex is null || trapCard.FieldIndex < 5)
            throw new BadRequestException("Trap card must be placed on your spell/trap field.");

        if (trapCard.Card is not TrapCard)
            throw new BadRequestException("Only trap cards can be activated as response.");

        var effect = trapCard.Card.Effect;
        if (effect is null)
        {
            await SendCardToGraveyard(trapCard, commandContext.UnitOfWork);
            return null;
        }

        var executionContext = new EffectExecutionContext(
            UnitOfWork: commandContext.UnitOfWork,
            Game: commandContext.Game,
            Turn: commandContext.CurrentTurn,
            ActivatingPlayer: defendingPlayer,
            SourceCard: trapCard,
            IsTrapResponse: true,
            AttackAttackerCard: attackerCard,
            AttackAttackerPlayer: attackingPlayer,
            AttackDefenderPlayer: defendingPlayer);

        var strategy = _strategyFactory.GetStrategy(effect.Type);
        await strategy.ApplyAsync(effect, executionContext, cancellationToken);
        await SendCardToGraveyard(trapCard, commandContext.UnitOfWork);

        var activation = await commandContext.UnitOfWork.EffectActivations.ActivateEffectAsync(
            commandContext.CurrentTurn,
            effect,
            trapCard);
        activation.Resolved = true;
        commandContext.UnitOfWork.EffectActivations.Update(activation);

        return new GameEffectActivationSummary(
            SourceCardId: trapCard.Id,
            ActivatedByPlayerGameId: defendingPlayer.Id,
            SourceCardName: trapCard.Card.Name,
            EffectType: effect.Type,
            IsTrap: true);
    }

    private static async Task SendCardToGraveyard(GameCard card, Backend.Data.UnitOfWork.IUnitOfWork unitOfWork)
    {
        var nextGraveOrder = await unitOfWork.GameCards.GetNextGraveOrderAsync(card.PlayerGameId);
        card.Zone = CardZone.Grave;
        card.FieldIndex = null;
        card.IsFaceDown = false;
        card.DefensePosition = false;
        card.DeckOrder = nextGraveOrder;
        unitOfWork.GameCards.Update(card);
    }
}
