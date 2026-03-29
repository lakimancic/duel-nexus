namespace Backend.Domain.Commands;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Domain.Engine.Phases;

public sealed record DrawActionCommand : IGameCommand<DrawActionResult>
{
    public async Task<DrawActionResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var drawsInTurn = await context.UnitOfWork.CardMovements
            .CountDrawsInTurnByPlayerAsync(context.CurrentTurn.Id, context.Actor.Id);

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
        var turnForResult = context.CurrentTurn;

        if (drawsAfterAction >= GameConstants.MaxDrawsPerTurn)
        {
            context.Actor.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(context.Actor);
            shouldEndTurn = true;
            var transition = await context.PhaseStateMachine.AdvanceAsync(
                new TurnPhaseStateContext(context.UnitOfWork, context.Game, context.CurrentTurn, context.Actor),
                TurnPhaseAdvanceTrigger.PlayerCompletedActions,
                cancellationToken);

            phaseAdvanced = transition.PhaseChanged || transition.TurnChanged;
            turnForResult = transition.Turn;
        }

        return new DrawActionResult(
            Game: context.Game,
            Turn: turnForResult,
            Player: context.Actor,
            DrawnCard: drawnCard,
            DrawsInTurn: drawsAfterAction,
            TurnEnded: shouldEndTurn,
            PhaseAdvanced: phaseAdvanced,
            CurrentPhase: turnForResult.Phase
        );
    }
}
