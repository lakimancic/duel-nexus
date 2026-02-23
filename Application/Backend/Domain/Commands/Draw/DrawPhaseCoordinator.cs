namespace Backend.Domain.Commands.Draw;

using Backend.Data.Enums;
using Backend.Domain.Engine;

public static class DrawPhaseCoordinator
{
    public static Task<bool> TryAdvanceToMain1Async(GameCommandContext context)
    {
        if (context.CurrentTurn.Phase != TurnPhase.Draw)
            return Task.FromResult(false);

        context.CurrentTurn.Phase = TurnPhase.Main1;
        context.UnitOfWork.Turns.Update(context.CurrentTurn);

        context.Actor.TurnEnded = false;
        context.UnitOfWork.PlayerGames.Update(context.Actor);

        return Task.FromResult(true);
    }
}
