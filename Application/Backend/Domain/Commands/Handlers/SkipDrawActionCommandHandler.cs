namespace Backend.Domain.Commands.Handlers;

using Backend.Data.Enums;
using Backend.Domain.Commands.Draw;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class SkipDrawActionCommandHandler : IGameCommandHandler<SkipDrawActionCommand, DrawPhaseProgressResult>
{
    public async Task<DrawPhaseProgressResult> HandleAsync(SkipDrawActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.CurrentTurn.Phase != TurnPhase.Draw)
            throw new BadRequestException("Skip draw is allowed only in Draw phase.");

        DrawActionValidator.EnsureActorOwnsCurrentTurn(context);

        if (!context.Actor.TurnEnded)
        {
            context.Actor.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(context.Actor);
        }

        var phaseAdvanced = await DrawPhaseCoordinator.TryAdvanceToMain1Async(context);

        return new DrawPhaseProgressResult(
            Game: context.Game,
            Turn: context.CurrentTurn,
            Player: context.Actor,
            TurnEnded: context.Actor.TurnEnded,
            PhaseAdvanced: phaseAdvanced,
            CurrentPhase: context.CurrentTurn.Phase
        );
    }
}
