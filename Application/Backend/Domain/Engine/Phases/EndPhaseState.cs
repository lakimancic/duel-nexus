namespace Backend.Domain.Engine.Phases;

using Backend.Data.Enums;
using Backend.Domain;
using Backend.Utils.WebApi;

public sealed class EndPhaseState : ITurnPhaseState
{
    private static readonly TimeSpan EndPhaseTimeout = TimeSpan.FromSeconds(GameConstants.EndPhaseTimeoutSeconds);

    public TurnPhase Phase => TurnPhase.End;
    public TimeSpan? Timeout => EndPhaseTimeout;

    public async Task<TurnPhaseTransitionResult> AdvanceAsync(
        TurnPhaseStateContext context,
        TurnPhaseAdvanceTrigger trigger,
        CancellationToken cancellationToken = default)
    {
        if (trigger != TurnPhaseAdvanceTrigger.Timeout)
            throw new BadRequestException("End phase advances automatically on timeout.");

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        context.Turn.EndedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.Turn);

        var nextTurn = await context.UnitOfWork.Turns.NextTurnAsync(context.Turn);
        nextTurn.ActivePlayerId = null;
        nextTurn.Phase = TurnPhase.Draw;
        nextTurn.StartedAt = DateTime.UtcNow;

        foreach (var player in players)
        {
            player.TurnEnded = player.LifePoints <= 0;
            context.UnitOfWork.PlayerGames.Update(player);
        }

        return new TurnPhaseTransitionResult(nextTurn, ActivePlayerId: null, TurnChanged: true, PhaseChanged: true);
    }
}
