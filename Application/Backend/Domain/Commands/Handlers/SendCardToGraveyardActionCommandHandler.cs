namespace Backend.Domain.Commands.Handlers;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class SendCardToGraveyardActionCommandHandler : IGameCommandHandler<SendCardToGraveyardActionCommand, GameCardUpdateResult>
{
    public async Task<GameCardUpdateResult> HandleAsync(SendCardToGraveyardActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.CurrentTurn.Phase != TurnPhase.Main1)
            throw new BadRequestException("Sending cards to graveyard is allowed only in Main1 phase.");

        if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
            throw new BadRequestException("Only active player can send cards to graveyard.");

        var card = await context.UnitOfWork.GameCards.GetByWithCardById(command.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        if (card.PlayerGameId != context.Actor.Id)
            throw new BadRequestException("You can move only your own cards.");

        if (card.Zone != CardZone.Hand)
            throw new BadRequestException("Only hand cards can be sent to graveyard.");

        card.Zone = CardZone.Grave;
        card.FieldIndex = null;
        card.IsFaceDown = false;
        card.DefensePosition = false;
        card.DeckOrder = null;
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
