namespace Backend.Domain.Commands;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Domain.Effects;
using Backend.Utils.WebApi;

public sealed record PlaceCardActionCommand(Guid GameCardId, int FieldIndex, bool FaceDown) : IGameCommand<PlaceCardResult>
{
    private static readonly CardEffectExecutor EffectExecutor = new();

    public async Task<PlaceCardResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var card = await context.UnitOfWork.GameCards.GetByWithCardById(GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        var shouldStartInDefense = FaceDown && card.Card.Type == CardType.Monster;
        card.Zone = CardZone.Field;
        card.FieldIndex = FieldIndex;
        card.IsFaceDown = FaceDown;
        card.DefensePosition = shouldStartInDefense;
        context.UnitOfWork.GameCards.Update(card);

        await context.UnitOfWork.PlaceCards.AddAsync(new PlaceCardAction
        {
            TurnId = context.CurrentTurn.Id,
            GameCardId = card.Id,
            FieldIndex = FieldIndex,
            FaceDown = FaceDown,
            DefensePosition = shouldStartInDefense,
            Type = ResolvePlaceType(card.Card.Type, FaceDown),
        });

        var activatedEffect = await EffectExecutor.TryActivateSpellOnPlacementAsync(
            context,
            card,
            RequestedTargetCardIds,
            RequestedTargetPlayerIds,
            cancellationToken);

        return new PlaceCardResult(
            Game: context.Game,
            Turn: context.CurrentTurn,
            Player: context.Actor,
            Card: card,
            FieldIndex: FieldIndex,
            FaceDown: FaceDown,
            CurrentPhase: context.CurrentTurn.Phase,
            ActivatedEffect: activatedEffect
        );
    }

    public IReadOnlyCollection<Guid>? RequestedTargetCardIds { get; init; }
    public IReadOnlyCollection<Guid>? RequestedTargetPlayerIds { get; init; }

    private static PlaceType ResolvePlaceType(CardType cardType, bool faceDown)
    {
        if (cardType == CardType.Monster)
            return PlaceType.NormalSummon;

        if (faceDown)
            return PlaceType.SetSpellTrap;

        return PlaceType.ActivateSpell;
    }
}
