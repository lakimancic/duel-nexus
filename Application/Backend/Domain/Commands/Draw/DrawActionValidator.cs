namespace Backend.Domain.Commands.Draw;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public static class DrawActionValidator
{
    public static void EnsureCanDraw(GameCommandContext context, int drawsInTurn, int handCount, int deckCount)
    {
        if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
            throw new BadRequestException("Only the active player can draw.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("Your turn has already ended.");

        if (context.CurrentTurn.Phase != TurnPhase.Draw)
            throw new BadRequestException("Draw action is allowed only in Draw phase.");

        if (drawsInTurn >= GameConstants.MaxDrawsPerTurn)
            throw new BadRequestException("You have already used both draw actions for this turn.");

        if (handCount >= GameConstants.MaxHandSize)
            throw new BadRequestException("Hand is full.");

        if (deckCount <= 0)
            throw new BadRequestException("Deck is empty.");
    }

    public static Guid? ResolveNextActivePlayer(IReadOnlyList<PlayerGame> players, Guid currentPlayerId)
    {
        if (players.Count == 0)
            return null;

        var currentIndex = -1;
        for (var i = 0; i < players.Count; i++)
        {
            if (players[i].Id == currentPlayerId)
            {
                currentIndex = i;
                break;
            }
        }

        if (currentIndex < 0)
            return null;

        return players[(currentIndex + 1) % players.Count].Id;
    }
}
