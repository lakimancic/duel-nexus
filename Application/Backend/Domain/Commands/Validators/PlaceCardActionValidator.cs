namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
using Backend.Domain;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class PlaceCardActionValidator : IGameCommandValidator<PlaceCardActionCommand, PlaceCardResult>
{
    public async Task ValidateAsync(PlaceCardActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.Game.FinishedAt is not null)
            throw new BadRequestException("Game is already finished.");

        if (context.Actor.LifePoints <= 0)
            throw new BadRequestException("You already lost this game.");

        if (context.CurrentTurn.Phase is not TurnPhase.Main1 and not TurnPhase.Main2)
            throw new BadRequestException("Placing cards is allowed only in Main1/Main2 phase.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("You already finished actions for this phase.");

        if (command.FieldIndex < 0 || command.FieldIndex > 9)
            throw new BadRequestException("Field index must be between 0 and 9.");

        var card = await context.UnitOfWork.GameCards.GetByWithCardById(command.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        if (card.PlayerGameId != context.Actor.Id)
            throw new BadRequestException("You can place only your own cards.");

        if (card.Zone != CardZone.Hand)
            throw new BadRequestException("Only hand cards can be placed.");

        var allCards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);
        var slotOccupied = allCards.Any(existing =>
            existing.PlayerGameId == context.Actor.Id &&
            existing.Zone == CardZone.Field &&
            existing.FieldIndex == command.FieldIndex);

        if (slotOccupied)
            throw new BadRequestException("Selected field slot is occupied.");

        var cardType = card.Card.Type;
        var isTopRow = command.FieldIndex <= 4;
        var isBottomRow = command.FieldIndex >= 5;

        if (cardType == CardType.Monster && !isTopRow)
            throw new BadRequestException("Monster cards can be placed only in top row.");

        if ((cardType == CardType.Spell || cardType == CardType.Trap) && !isBottomRow)
            throw new BadRequestException("Spell/Trap cards can be placed only in bottom row.");

        if (context.CurrentTurn.Phase == TurnPhase.Main2 && cardType == CardType.Monster)
            throw new BadRequestException("Monster cards cannot be placed in Main2 phase.");

        if (command.FaceDown)
        {
            if (cardType == CardType.Spell)
                throw new BadRequestException("Spell cards cannot be placed face-down.");

            if (cardType == CardType.Monster && !isTopRow)
                throw new BadRequestException("Face-down monsters must be placed in monster zone.");
        }

        var phasePlacements = await context.UnitOfWork.PlaceCards.GetByTurnAndPlayerSinceAsync(
            context.CurrentTurn.Id,
            context.Actor.Id,
            context.CurrentTurn.StartedAt);

        var monstersPlaced = phasePlacements.Count(place =>
            place.Card.Card.Type == CardType.Monster &&
            place.Type == PlaceType.NormalSummon);

        var spellsPlaced = phasePlacements.Count(place =>
            place.Card.Card.Type == CardType.Spell &&
            (place.Type == PlaceType.ActivateSpell || place.Type == PlaceType.SetSpellTrap));

        var trapsPlaced = phasePlacements.Count(place =>
            place.Card.Card.Type == CardType.Trap &&
            place.Type == PlaceType.SetSpellTrap);

        if (cardType == CardType.Monster)
        {
            var maxMonsters = context.CurrentTurn.Phase == TurnPhase.Main1
                ? GameConstants.MaxMonsterPlacementsInMain1
                : GameConstants.MaxMonsterPlacementsInMain2;
            if (monstersPlaced >= maxMonsters)
                throw new BadRequestException("Monster placement limit reached for this phase.");
        }

        if (cardType == CardType.Spell && spellsPlaced >= GameConstants.MaxSpellPlacementsPerMainPhase)
            throw new BadRequestException("Spell placement limit reached for this phase.");

        if (cardType == CardType.Trap && trapsPlaced >= GameConstants.MaxTrapPlacementsPerMainPhase)
            throw new BadRequestException("Trap placement limit reached for this phase.");
    }
}
