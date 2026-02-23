namespace Backend.Domain.Commands.Handlers;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class AdvancePhaseActionCommandHandler : IGameCommandHandler<AdvancePhaseActionCommand, PhaseAdvanceResult>
{
    public async Task<PhaseAdvanceResult> HandleAsync(AdvancePhaseActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
            throw new BadRequestException("Only active player can advance phase.");

        var turnChanged = false;
        var activePlayerId = context.Actor.Id;
        var turnForResult = context.CurrentTurn;

        switch (context.CurrentTurn.Phase)
        {
            case TurnPhase.Draw:
                context.CurrentTurn.Phase = TurnPhase.Main1;
                context.UnitOfWork.Turns.Update(context.CurrentTurn);
                break;

            case TurnPhase.Main1:
            case TurnPhase.End:
                turnForResult = await AdvanceToNextPlayerDrawTurn(context);
                turnChanged = true;
                activePlayerId = turnForResult.ActivePlayerId
                    ?? throw new InvalidOperationException("Active player must be set for new turn.");
                break;

            case TurnPhase.Battle:
                context.CurrentTurn.Phase = TurnPhase.Main2;
                context.UnitOfWork.Turns.Update(context.CurrentTurn);
                break;

            case TurnPhase.Main2:
                context.CurrentTurn.Phase = TurnPhase.End;
                context.UnitOfWork.Turns.Update(context.CurrentTurn);
                break;

            default:
                throw new BadRequestException("Unsupported phase transition.");
        }

        return new PhaseAdvanceResult(
            Game: context.Game,
            Turn: turnForResult,
            Player: context.Actor,
            ActivePlayerId: activePlayerId,
            CurrentPhase: turnForResult.Phase,
            TurnChanged: turnChanged
        );
    }

    private static async Task<Turn> AdvanceToNextPlayerDrawTurn(GameCommandContext context)
    {
        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        var currentActivePlayerId = context.CurrentTurn.ActivePlayerId
            ?? throw new BadRequestException("Active player is not set.");

        var currentIndex = players.FindIndex(player => player.Id == currentActivePlayerId);
        if (currentIndex < 0)
            throw new BadRequestException("Active player not found in game players.");

        var nextPlayer = players[(currentIndex + 1) % players.Count];

        context.CurrentTurn.EndedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.CurrentTurn);

        var nextTurn = await context.UnitOfWork.Turns.NextTurnAsync(context.CurrentTurn);
        nextTurn.ActivePlayerId = nextPlayer.Id;
        nextTurn.Phase = TurnPhase.Draw;

        foreach (var player in players)
        {
            player.TurnEnded = false;
            context.UnitOfWork.PlayerGames.Update(player);
        }

        return nextTurn;
    }
}
