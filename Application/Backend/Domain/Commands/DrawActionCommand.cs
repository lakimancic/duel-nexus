namespace Backend.Domain.Commands;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Commands.Draw;
using Backend.Domain.Engine;

public sealed class DrawActionCommand : IGameCommand<DrawActionResult>
{
    public async Task<DrawActionResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
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
        var shouldEndTurn = drawsAfterAction >= GameConstants.MaxDrawsPerTurn;
        Guid? nextActivePlayerId = null;

        if (shouldEndTurn)
            nextActivePlayerId = await EndTurnAfterSecondDrawAsync(context, cancellationToken);

        return new DrawActionResult(
            Game: context.Game,
            Turn: context.CurrentTurn,
            Player: context.Actor,
            DrawnCard: drawnCard,
            DrawsInTurn: drawsAfterAction,
            TurnEnded: shouldEndTurn,
            NextActivePlayerId: nextActivePlayerId
        );
    }

    private static async Task<Guid?> EndTurnAfterSecondDrawAsync(GameCommandContext context, CancellationToken cancellationToken)
    {
        context.Actor.TurnEnded = true;
        context.CurrentTurn.EndedAt = DateTime.UtcNow;

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);

        var nextActivePlayerId = DrawActionValidator.ResolveNextActivePlayer(players, context.Actor.Id);
        if (!nextActivePlayerId.HasValue)
            return null;

        foreach (var player in players)
        {
            player.TurnEnded = false;
            context.UnitOfWork.PlayerGames.Update(player);
        }

        await context.UnitOfWork.Turns.AddAsync(new Turn
        {
            GameId = context.Game.Id,
            TurnNumber = context.CurrentTurn.TurnNumber + 1,
            ActivePlayerId = nextActivePlayerId,
            Phase = TurnPhase.Draw,
            StartedAt = DateTime.UtcNow,
        });

        return nextActivePlayerId;
    }
}
