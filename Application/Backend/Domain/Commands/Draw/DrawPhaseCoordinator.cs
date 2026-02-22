namespace Backend.Domain.Commands.Draw;

using Backend.Data.Enums;
using Backend.Domain.Engine;

public static class DrawPhaseCoordinator
{
    public static async Task<bool> TryAdvanceToMain1Async(GameCommandContext context)
    {
        if (context.CurrentTurn.Phase != TurnPhase.Draw)
            return false;

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            return false;

        if (!players.All(player => player.TurnEnded))
            return false;

        context.CurrentTurn.Phase = TurnPhase.Main1;
        context.UnitOfWork.Turns.Update(context.CurrentTurn);

        foreach (var player in players)
        {
            player.TurnEnded = false;
            context.UnitOfWork.PlayerGames.Update(player);
        }

        return true;
    }
}
