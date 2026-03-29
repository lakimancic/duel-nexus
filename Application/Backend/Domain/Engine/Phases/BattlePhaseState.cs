namespace Backend.Domain.Engine.Phases;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Utils.WebApi;

public sealed class BattlePhaseState : ITurnPhaseState
{
    public TurnPhase Phase => TurnPhase.Battle;
    public TimeSpan? Timeout => null;

    public async Task<TurnPhaseTransitionResult> AdvanceAsync(
        TurnPhaseStateContext context,
        TurnPhaseAdvanceTrigger trigger,
        CancellationToken cancellationToken = default)
    {
        if (trigger != TurnPhaseAdvanceTrigger.PlayerAdvance)
            throw new BadRequestException("Unsupported trigger for Battle phase.");

        var actor = context.Actor
            ?? throw new BadRequestException("Actor is required for this phase transition.");

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        MarkActorAsEndedIfNeeded(actor, context);

        var nextBattlePlayer = GetNextNotEndedPlayer(players, actor.Id);
        if (nextBattlePlayer != null)
        {
            context.Turn.ActivePlayerId = nextBattlePlayer.Id;
            context.UnitOfWork.Turns.Update(context.Turn);
            return new TurnPhaseTransitionResult(context.Turn, nextBattlePlayer.Id, TurnChanged: false, PhaseChanged: false);
        }

        context.Turn.EndedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.Turn);

        var nextTurn = await context.UnitOfWork.Turns.NextTurnAsync(context.Turn);
        nextTurn.ActivePlayerId = null;
        nextTurn.Phase = TurnPhase.Draw;
        nextTurn.StartedAt = DateTime.UtcNow;

        ResetTurnEndedFlags(players, context);

        return new TurnPhaseTransitionResult(nextTurn, ActivePlayerId: null, TurnChanged: true, PhaseChanged: true);
    }

    private static void MarkActorAsEndedIfNeeded(PlayerGame actor, TurnPhaseStateContext context)
    {
        if (actor.TurnEnded)
            return;

        actor.TurnEnded = true;
        context.UnitOfWork.PlayerGames.Update(actor);
    }

    private static PlayerGame? GetNextNotEndedPlayer(List<PlayerGame> players, Guid currentPlayerId)
    {
        var currentIndex = players.FindIndex(player => player.Id == currentPlayerId);
        if (currentIndex < 0)
            throw new BadRequestException("Current battle player not found in game players.");

        for (var step = 1; step < players.Count; step++)
        {
            var candidate = players[(currentIndex + step) % players.Count];
            if (!candidate.TurnEnded)
                return candidate;
        }

        return null;
    }

    private static void ResetTurnEndedFlags(IEnumerable<PlayerGame> players, TurnPhaseStateContext context)
    {
        foreach (var player in players)
        {
            player.TurnEnded = false;
            context.UnitOfWork.PlayerGames.Update(player);
        }
    }
}
