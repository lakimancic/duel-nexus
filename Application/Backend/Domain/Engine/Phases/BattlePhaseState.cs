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
        if (trigger is not TurnPhaseAdvanceTrigger.PlayerAdvance and not TurnPhaseAdvanceTrigger.PlayerCompletedActions)
            throw new BadRequestException("Unsupported trigger for Battle phase.");

        var actor = context.Actor
            ?? throw new BadRequestException("Actor is required for this phase transition.");

        var players = await context.UnitOfWork.PlayerGames.GetByGameIdOrderedAsync(context.Game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        MarkActorAsEndedIfNeeded(actor, context);
        await MarkPlayersWithoutAvailableAttacksAsEndedAsync(players, context, cancellationToken);

        var alivePlayers = players.Where(player => player.LifePoints > 0).ToList();
        var nextBattlePlayer = GetNextNotEndedPlayer(alivePlayers, actor.Id);
        if (nextBattlePlayer != null)
        {
            context.Turn.ActivePlayerId = nextBattlePlayer.Id;
            context.UnitOfWork.Turns.Update(context.Turn);
            return new TurnPhaseTransitionResult(context.Turn, nextBattlePlayer.Id, TurnChanged: false, PhaseChanged: false);
        }

        context.Turn.Phase = TurnPhase.Main2;
        context.Turn.ActivePlayerId = null;
        context.Turn.StartedAt = DateTime.UtcNow;
        context.UnitOfWork.Turns.Update(context.Turn);

        ResetTurnEndedFlags(players, context);

        return new TurnPhaseTransitionResult(context.Turn, ActivePlayerId: null, TurnChanged: false, PhaseChanged: true);
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
        if (players.Count == 0)
            return null;

        var currentIndex = players.FindIndex(player => player.Id == currentPlayerId);
        if (currentIndex < 0)
            currentIndex = 0;

        for (var step = 1; step <= players.Count; step++)
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
            player.TurnEnded = player.LifePoints <= 0;
            context.UnitOfWork.PlayerGames.Update(player);
        }
    }

    private static async Task MarkPlayersWithoutAvailableAttacksAsEndedAsync(
        IEnumerable<PlayerGame> players,
        TurnPhaseStateContext context,
        CancellationToken cancellationToken)
    {
        var allCards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);
        var attackedCardIds = await context.UnitOfWork.Attacks.GetAttackerCardIdsByTurnAsync(context.Turn.Id);

        foreach (var player in players)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (player.LifePoints <= 0)
            {
                if (!player.TurnEnded)
                {
                    player.TurnEnded = true;
                    context.UnitOfWork.PlayerGames.Update(player);
                }

                continue;
            }

            var hasAvailableAttacker = allCards.Any(card =>
                card.PlayerGameId == player.Id &&
                card.Zone == CardZone.Field &&
                card.FieldIndex is >= 0 and <= 4 &&
                !card.IsFaceDown &&
                !card.DefensePosition &&
                card.Card is MonsterCard &&
                !attackedCardIds.Contains(card.Id));

            if (hasAvailableAttacker || player.TurnEnded)
                continue;

            player.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(player);
        }
    }
}
