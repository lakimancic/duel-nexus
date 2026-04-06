namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
using Backend.Domain.Commands;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class DrawActionValidator : IGameCommandValidator<DrawActionCommand, DrawActionResult>
{
    public async Task ValidateAsync(DrawActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.Game.FinishedAt is not null)
            throw new BadRequestException("Game is already finished.");

        if (context.Actor.LifePoints <= 0)
            throw new BadRequestException("You already lost this game.");

        var drawsInTurn = await context.UnitOfWork.CardMovements
            .CountDrawsInTurnByPlayerAsync(context.CurrentTurn.Id, context.Actor.Id);

        var handCount = await context.UnitOfWork.GameCards
            .CountByPlayerAndZoneAsync(context.Actor.Id, CardZone.Hand);

        var deckCount = await context.UnitOfWork.GameCards
            .CountByPlayerAndZoneAsync(context.Actor.Id, CardZone.Deck);

        if (context.CurrentTurn.Phase != TurnPhase.Draw)
            throw new BadRequestException("Draw action is allowed only in Draw phase.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("You already finished draw actions for this phase.");

        if (drawsInTurn >= GameConstants.MaxDrawsPerTurn)
            throw new BadRequestException("You have already used both draw actions for this phase.");

        if (handCount >= GameConstants.MaxHandSize)
            throw new BadRequestException("Hand is full.");

        if (deckCount <= 0)
            throw new BadRequestException("Deck is empty.");
    }
}
