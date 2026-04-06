namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
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

        if (context.CurrentTurn.Phase != TurnPhase.Main1)
            throw new BadRequestException("Placing cards is allowed only in Main1 phase.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("You already finished Main1 actions for this phase.");

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
    }
}
