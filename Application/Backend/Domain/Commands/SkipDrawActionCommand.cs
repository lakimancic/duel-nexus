namespace Backend.Domain.Commands;

using Backend.Domain.Commands.Draw;
using Backend.Domain.Engine;

public sealed record SkipDrawActionCommand : IGameCommand<DrawPhaseProgressResult>
{
    public async Task<DrawPhaseProgressResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
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
