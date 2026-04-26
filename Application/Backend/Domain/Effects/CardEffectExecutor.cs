namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class CardEffectExecutor
{
    public async Task<GameEffectActivationSummary?> TryActivateSpellOnPlacementAsync(
        GameCommandContext commandContext,
        GameCard placedCard,
        IReadOnlyCollection<Guid>? requestedTargetCardIds,
        IReadOnlyCollection<Guid>? requestedTargetPlayerIds,
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
            AttackDefenderPlayer: null,
            RequestedTargetCardIds: requestedTargetCardIds,
            RequestedTargetPlayerIds: requestedTargetPlayerIds,
            AppliedTargetCardIds: [],
            AppliedTargetPlayerIds: []);

        var strategy = EffectStrategyFactory.CreateStrategy(effect.Type);
        await strategy.ApplyAsync(effect, executionContext, cancellationToken);
        await SendCardToGraveyard(placedCard, commandContext.UnitOfWork);

        var activation = await commandContext.UnitOfWork.EffectActivations.ActivateEffectAsync(
            commandContext.CurrentTurn,
            effect,
            placedCard);
        activation.Resolved = true;
        await SaveTargetsAsync(activation, executionContext, commandContext.UnitOfWork);

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
        IReadOnlyCollection<Guid>? requestedTargetCardIds,
        IReadOnlyCollection<Guid>? requestedTargetPlayerIds,
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
            AttackDefenderPlayer: defendingPlayer,
            RequestedTargetCardIds: requestedTargetCardIds,
            RequestedTargetPlayerIds: requestedTargetPlayerIds,
            AppliedTargetCardIds: [],
            AppliedTargetPlayerIds: []);

        var strategy = EffectStrategyFactory.CreateStrategy(effect.Type);
        await strategy.ApplyAsync(effect, executionContext, cancellationToken);
        await SendCardToGraveyard(trapCard, commandContext.UnitOfWork);

        var activation = await commandContext.UnitOfWork.EffectActivations.ActivateEffectAsync(
            commandContext.CurrentTurn,
            effect,
            trapCard);
        activation.Resolved = true;
        await SaveTargetsAsync(activation, executionContext, commandContext.UnitOfWork);

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

    private static async Task SaveTargetsAsync(
        EffectActivation activation,
        EffectExecutionContext context,
        Backend.Data.UnitOfWork.IUnitOfWork unitOfWork)
    {
        if (context.AppliedTargetCardIds.Count == 0 && context.AppliedTargetPlayerIds.Count == 0)
            return;

        var allCards = await unitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);
        var cardById = allCards.ToDictionary(card => card.Id, card => card);
        var createdTargets = new HashSet<(Guid? TargetCardId, Guid TargetPlayerId)>();

        foreach (var targetCardId in context.AppliedTargetCardIds)
        {
            if (!cardById.TryGetValue(targetCardId, out var targetCard))
                continue;

            var key = (TargetCardId: (Guid?)targetCardId, TargetPlayerId: targetCard.PlayerGameId);
            if (!createdTargets.Add(key))
                continue;

            await unitOfWork.EffectTargets.AddAsync(new EffectTarget
            {
                Activation = activation,
                TargetCardId = targetCardId,
                TargetPlayerId = targetCard.PlayerGameId,
            });
        }

        var playersCoveredByCardTargets = context.AppliedTargetCardIds
            .Where(cardById.ContainsKey)
            .Select(cardId => cardById[cardId].PlayerGameId)
            .ToHashSet();

        foreach (var targetPlayerId in context.AppliedTargetPlayerIds.Where(playerId => !playersCoveredByCardTargets.Contains(playerId)))
        {
            var key = (TargetCardId: (Guid?)null, TargetPlayerId: targetPlayerId);
            if (!createdTargets.Add(key))
                continue;

            await unitOfWork.EffectTargets.AddAsync(new EffectTarget
            {
                Activation = activation,
                TargetCardId = null,
                TargetPlayerId = targetPlayerId,
            });
        }
    }
}
