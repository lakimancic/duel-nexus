namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class SkipDrawActionValidator : IGameCommandValidator<SkipDrawActionCommand, DrawPhaseProgressResult>
{
    public Task ValidateAsync(SkipDrawActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.Game.FinishedAt is not null)
            throw new BadRequestException("Game is already finished.");

        if (context.Actor.LifePoints <= 0)
            throw new BadRequestException("You already lost this game.");

        if (context.CurrentTurn.Phase != TurnPhase.Draw)
            throw new BadRequestException("Skip draw is allowed only in Draw phase.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("You already finished draw actions for this phase.");

        return Task.CompletedTask;
    }
}
