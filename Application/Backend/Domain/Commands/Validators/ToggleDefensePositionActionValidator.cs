namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class ToggleDefensePositionActionValidator : IGameCommandValidator<ToggleDefensePositionActionCommand, GameCardUpdateResult>
{
    public async Task ValidateAsync(ToggleDefensePositionActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.Game.FinishedAt is not null)
            throw new BadRequestException("Game is already finished.");

        if (context.Actor.LifePoints <= 0)
            throw new BadRequestException("You already lost this game.");

        if (context.CurrentTurn.Phase is not TurnPhase.Main1 and not TurnPhase.Main2)
            throw new BadRequestException("Changing battle position is allowed only in Main1/Main2 phase.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("You already finished actions for this phase.");

        var card = await context.UnitOfWork.GameCards.GetByWithCardById(command.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        if (card.PlayerGameId != context.Actor.Id)
            throw new BadRequestException("You can update only your own cards.");

        if (card.Zone != CardZone.Field)
            throw new BadRequestException("Only field cards can change battle position.");

        if (card.Card is not MonsterCard || card.FieldIndex is null || card.FieldIndex > 4)
            throw new BadRequestException("Only monster field cards can change battle position.");
    }
}
