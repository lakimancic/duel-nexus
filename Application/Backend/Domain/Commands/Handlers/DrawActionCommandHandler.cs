namespace Backend.Domain.Commands.Handlers;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Commands.Draw;
using Backend.Domain.Engine;

public sealed class DrawActionCommandHandler : IGameCommandHandler<DrawActionCommand, DrawActionResult>
{
    public async Task<DrawActionResult> HandleAsync(DrawActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var drawsInTurn = await context.UnitOfWork.CardMovements
            .CountDrawsInTurnByPlayerAsync(context.CurrentTurn.Id, context.Actor.Id);

        var handCount = await context.UnitOfWork.GameCards
            .CountByPlayerAndZoneAsync(context.Actor.Id, CardZone.Hand);

        var deckCount = await context.UnitOfWork.GameCards
            .CountByPlayerAndZoneAsync(context.Actor.Id, CardZone.Deck);

        DrawActionValidator.EnsureCanDraw(context, drawsInTurn, handCount, deckCount);

        var drawnCard = await context.UnitOfWork.GameCards
            .GetTopDeckCardByPlayerWithCardAsync(context.Actor.Id)
            ?? throw new InvalidOperationException("Deck card expected but not found.");

        drawnCard.Zone = CardZone.Hand;
        drawnCard.DeckOrder = null;
        context.UnitOfWork.GameCards.Update(drawnCard);

        await context.UnitOfWork.CardMovements.AddAsync(new CardMovementAction
        {
            TurnId = context.CurrentTurn.Id,
            GameCardId = drawnCard.Id,
            FromZone = CardZone.Deck,
            ToZone = CardZone.Hand,
            MovementType = CardMovementType.Draw,
        });

        var drawsAfterAction = drawsInTurn + 1;
        var phaseAdvanced = false;
        var shouldEndTurn = context.Actor.TurnEnded;

        if (drawsAfterAction >= GameConstants.MaxDrawsPerTurn)
        {
            context.Actor.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(context.Actor);
            shouldEndTurn = true;
            phaseAdvanced = await DrawPhaseCoordinator.TryAdvanceToMain1Async(context);        
        }       
         
        return new DrawActionResult(
            Game: context.Game,
            Turn: context.CurrentTurn,
            Player: context.Actor,
            DrawnCard: drawnCard,
            DrawsInTurn: drawsAfterAction,
            TurnEnded: shouldEndTurn,
            PhaseAdvanced: phaseAdvanced,
            CurrentPhase: context.CurrentTurn.Phase
        );
    }
}
