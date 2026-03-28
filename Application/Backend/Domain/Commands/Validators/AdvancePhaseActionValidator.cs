namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class AdvancePhaseActionValidator : IGameCommandValidator<AdvancePhaseActionCommand, PhaseAdvanceResult>
{
    public Task ValidateAsync(AdvancePhaseActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        switch (context.CurrentTurn.Phase)
        {
            case TurnPhase.Draw:
            case TurnPhase.Main1:
                if (context.Actor.TurnEnded)
                    throw new BadRequestException("You already clicked Next in this phase.");
                break;

            case TurnPhase.Battle:
                if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
                    throw new BadRequestException("Only active player can advance Battle phase.");
                break;

            default:
                throw new BadRequestException("Unsupported phase transition.");
        }

        return Task.CompletedTask;
    }
}
