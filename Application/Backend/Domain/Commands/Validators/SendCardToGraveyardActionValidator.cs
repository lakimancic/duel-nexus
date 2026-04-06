namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class SendCardToGraveyardActionValidator : IGameCommandValidator<SendCardToGraveyardActionCommand, GameCardUpdateResult>
{
    public async Task ValidateAsync(SendCardToGraveyardActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.Game.FinishedAt is not null)
            throw new BadRequestException("Game is already finished.");

        if (context.Actor.LifePoints <= 0)
            throw new BadRequestException("You already lost this game.");

        if (context.CurrentTurn.Phase != TurnPhase.Main1)
            throw new BadRequestException("Sending cards to graveyard is allowed only in Main1 phase.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("You already finished Main1 actions for this phase.");

        var card = await context.UnitOfWork.GameCards.GetByWithCardById(command.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        if (card.PlayerGameId != context.Actor.Id)
            throw new BadRequestException("You can move only your own cards.");

        if (card.Zone != CardZone.Hand)
            throw new BadRequestException("Only hand cards can be sent to graveyard.");
    }
}
