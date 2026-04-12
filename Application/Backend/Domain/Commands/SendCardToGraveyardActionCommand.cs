namespace Backend.Domain.Commands;

using Backend.Data.Enums;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed record SendCardToGraveyardActionCommand(Guid GameCardId) : IGameCommand<GameCardUpdateResult>
{
    public async Task<GameCardUpdateResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var card = await context.UnitOfWork.GameCards.GetByWithCardById(GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");
        var nextGraveOrder = await context.UnitOfWork.GameCards.GetNextGraveOrderAsync(card.PlayerGameId);

        card.Zone = CardZone.Grave;
        card.FieldIndex = null;
        card.IsFaceDown = false;
        card.DefensePosition = false;
        card.DeckOrder = nextGraveOrder;
        context.UnitOfWork.GameCards.Update(card);

        return new GameCardUpdateResult(
            Game: context.Game,
            Turn: context.CurrentTurn,
            Player: context.Actor,
            Card: card,
            CurrentPhase: context.CurrentTurn.Phase
        );
    }
}
