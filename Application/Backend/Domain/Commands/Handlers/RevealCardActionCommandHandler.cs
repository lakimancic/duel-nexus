namespace Backend.Domain.Commands.Handlers;

using Backend.Data.Enums;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class RevealCardActionCommandHandler : IGameCommandHandler<RevealCardActionCommand, GameCardUpdateResult>
{
    public async Task<GameCardUpdateResult> HandleAsync(RevealCardActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.CurrentTurn.Phase != TurnPhase.Main1)
            throw new BadRequestException("Revealing cards is allowed only in Main1 phase.");

        if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
            throw new BadRequestException("Only active player can reveal cards.");

        var card = await context.UnitOfWork.GameCards.GetByWithCardById(command.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        if (card.PlayerGameId != context.Actor.Id)
            throw new BadRequestException("You can reveal only your own cards.");

        if (card.Zone != CardZone.Field)
            throw new BadRequestException("Only field cards can be revealed.");

        if (!card.IsFaceDown)
            throw new BadRequestException("Card is already face-up.");

        card.IsFaceDown = false;
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
