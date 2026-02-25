namespace Backend.Domain.Commands.Draw;

using Backend.Data.Enums;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public static class DrawActionValidator
{
    public static void EnsureCanDraw(GameCommandContext context, int drawsInTurn, int handCount, int deckCount)
    {
        if (context.CurrentTurn.Phase != TurnPhase.Draw)
            throw new BadRequestException("Draw action is allowed only in Draw phase.");

        if (context.Actor.TurnEnded)
            throw new BadRequestException("You already finished draw actions for this phase.");

        if (drawsInTurn >= GameConstants.MaxDrawsPerTurn)
            throw new BadRequestException("You have already used both draw actions for this phase.");

        if (handCount >= GameConstants.MaxHandSize)
            throw new BadRequestException("Hand is full.");

        if (deckCount <= 0)
            throw new BadRequestException("Deck is empty.");
    }
}
