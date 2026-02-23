namespace Backend.Domain.Commands.Handlers;

using Backend.Data.Enums;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class ToggleDefensePositionActionCommandHandler : IGameCommandHandler<ToggleDefensePositionActionCommand, GameCardUpdateResult>
{
    public async Task<GameCardUpdateResult> HandleAsync(ToggleDefensePositionActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.CurrentTurn.Phase != TurnPhase.Main1)
            throw new BadRequestException("Changing battle position is allowed only in Main1 phase.");

        if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
            throw new BadRequestException("Only active player can change card position.");

        var card = await context.UnitOfWork.GameCards.GetByWithCardById(command.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        if (card.PlayerGameId != context.Actor.Id)
            throw new BadRequestException("You can update only your own cards.");

        if (card.Zone != CardZone.Field)
            throw new BadRequestException("Only field cards can change battle position.");

        card.DefensePosition = !card.DefensePosition;
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
