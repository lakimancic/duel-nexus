namespace Backend.Domain.Commands;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Domain.Effects;
using Backend.Domain.Engine.Phases;
using Backend.Utils.WebApi;

public sealed record AttackActionCommand(
    Guid AttackerCardId,
    Guid? DefenderCardId,
    Guid? DefenderPlayerGameId,
    Guid? ActivatedTrapCardId) : IGameCommand<BattleAttackResult>
{
    private static readonly CardEffectExecutor EffectExecutor = new();

    public async Task<BattleAttackResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var attacker = await context.UnitOfWork.GameCards.GetByWithCardById(AttackerCardId)
            ?? throw new ObjectNotFoundException("Attacker card not found.");

        GameCard? defenderCard = null;
        if (DefenderCardId.HasValue)
        {
            defenderCard = await context.UnitOfWork.GameCards.GetByWithCardById(DefenderCardId.Value)
                ?? throw new ObjectNotFoundException("Defender card not found.");
        }

        PlayerGame? directDefenderPlayer = null;
        if (DefenderPlayerGameId.HasValue)
        {
            directDefenderPlayer = await context.UnitOfWork.PlayerGames.GetByIdAsync(DefenderPlayerGameId.Value)
                ?? throw new ObjectNotFoundException("Defender player not found.");
        }

        var defenderPlayer = directDefenderPlayer;
        if (defenderPlayer is null && defenderCard is not null)
        {
            defenderPlayer = await context.UnitOfWork.PlayerGames.GetByIdAsync(defenderCard.PlayerGameId)
                ?? throw new ObjectNotFoundException("Defender player not found.");
        }

        if (defenderPlayer is null)
            throw new BadRequestException("Defender player cannot be resolved.");

        var activatedEffect = await EffectExecutor.TryActivateTrapDuringAttackAsync(
            commandContext: context,
            defendingPlayer: defenderPlayer,
            attackingPlayer: context.Actor,
            attackerCard: attacker,
            trapCardId: ActivatedTrapCardId,
            cancellationToken: cancellationToken);

        if (attacker.Zone != CardZone.Field || attacker.FieldIndex is null || attacker.IsFaceDown || attacker.DefensePosition)
        {
            await context.UnitOfWork.Attacks.AddAsync(new AttackAction
            {
                TurnId = context.CurrentTurn.Id,
                AttackerCardId = attacker.Id,
                DefenderCardId = defenderCard?.Id,
                DefenderPlayerGameId = directDefenderPlayer?.Id ?? defenderPlayer.Id,
                ExecutedAt = DateTime.UtcNow,
            });

            return new BattleAttackResult(
                Game: context.Game,
                Turn: context.CurrentTurn,
                Player: context.Actor,
                AttackerCardId: attacker.Id,
                DefenderCardId: defenderCard?.Id,
                DefenderPlayerGameId: directDefenderPlayer?.Id ?? defenderPlayer.Id,
                DamageToDefender: 0,
                DamageToAttacker: 0,
                AttackerDestroyed: true,
                DefenderDestroyed: false,
                AttackFailed: true,
                PhaseAdvanced: false,
                TurnChanged: false,
                ActivePlayerId: context.CurrentTurn.ActivePlayerId,
                CurrentPhase: context.CurrentTurn.Phase,
                ActivatedEffect: activatedEffect
            );
        }

        var attackerMonster = attacker.Card as MonsterCard
            ?? throw new BadRequestException("Only monster cards can attack.");

        var attackerAttack = attackerMonster.Attack;
        var attackerDestroyed = false;
        var defenderDestroyed = false;
        var damageToDefender = 0;
        var damageToAttacker = 0;
        var attackFailed = false;

        if (defenderCard is null)
        {
            damageToDefender = Math.Max(0, attackerAttack);
        }
        else
        {
            var defenderMonster = defenderCard.Card as MonsterCard
                ?? throw new BadRequestException("Only monster cards can be attacked.");
            var defenderStat = defenderCard.DefensePosition
                ? defenderMonster.Defense
                : defenderMonster.Attack;

            if (attackerAttack > defenderStat)
            {
                defenderDestroyed = true;
                if (!defenderCard.DefensePosition)
                    damageToDefender = attackerAttack - defenderStat;
            }
            else if (attackerAttack < defenderStat)
            {
                attackerDestroyed = true;
                damageToAttacker = defenderStat - attackerAttack;
                attackFailed = true;
            }
            else if (!defenderCard.DefensePosition)
            {
                attackerDestroyed = true;
                defenderDestroyed = true;
            }
        }

        if (damageToDefender > 0)
        {
            defenderPlayer.LifePoints = Math.Max(0, defenderPlayer.LifePoints - damageToDefender);
            if (defenderPlayer.LifePoints == 0)
                defenderPlayer.TurnEnded = true;

            context.UnitOfWork.PlayerGames.Update(defenderPlayer);
        }

        if (damageToAttacker > 0)
        {
            context.Actor.LifePoints = Math.Max(0, context.Actor.LifePoints - damageToAttacker);
            if (context.Actor.LifePoints == 0)
                context.Actor.TurnEnded = true;

            context.UnitOfWork.PlayerGames.Update(context.Actor);
        }

        if (attackerDestroyed)
            await SendCardToGraveyard(attacker, context);

        if (defenderDestroyed && defenderCard is not null)
            await SendCardToGraveyard(defenderCard, context);

        await context.UnitOfWork.Attacks.AddAsync(new AttackAction
        {
            TurnId = context.CurrentTurn.Id,
            AttackerCardId = attacker.Id,
            DefenderCardId = defenderCard?.Id,
            DefenderPlayerGameId = directDefenderPlayer?.Id ?? defenderPlayer.Id,
            ExecutedAt = DateTime.UtcNow,
        });

        var turnForResult = context.CurrentTurn;
        var phaseAdvanced = false;
        var turnChanged = false;
        var activePlayerId = context.CurrentTurn.ActivePlayerId;

        var hasMoreAttacks = await HasAvailableAttackerAsync(
            context,
            context.Actor.Id,
            additionalAttackedCardIds: [attacker.Id],
            cancellationToken);
        if (!hasMoreAttacks && !context.Actor.TurnEnded)
        {
            context.Actor.TurnEnded = true;
            context.UnitOfWork.PlayerGames.Update(context.Actor);
        }

        if (!hasMoreAttacks || context.Actor.LifePoints <= 0)
        {
            var transition = await context.PhaseStateMachine.AdvanceAsync(
                new TurnPhaseStateContext(context.UnitOfWork, context.Game, context.CurrentTurn, context.Actor),
                TurnPhaseAdvanceTrigger.PlayerCompletedActions,
                cancellationToken);

            turnForResult = transition.Turn;
            phaseAdvanced = transition.PhaseChanged;
            turnChanged = transition.TurnChanged;
            activePlayerId = transition.ActivePlayerId;
        }

        return new BattleAttackResult(
            Game: context.Game,
            Turn: turnForResult,
            Player: context.Actor,
            AttackerCardId: attacker.Id,
            DefenderCardId: defenderCard?.Id,
            DefenderPlayerGameId: directDefenderPlayer?.Id ?? defenderPlayer.Id,
            DamageToDefender: damageToDefender,
            DamageToAttacker: damageToAttacker,
            AttackerDestroyed: attackerDestroyed,
            DefenderDestroyed: defenderDestroyed,
            AttackFailed: attackFailed,
            PhaseAdvanced: phaseAdvanced,
            TurnChanged: turnChanged,
            ActivePlayerId: activePlayerId,
            CurrentPhase: turnForResult.Phase,
            ActivatedEffect: activatedEffect
        );
    }

    private static async Task SendCardToGraveyard(GameCard card, GameCommandContext context)
    {
        var nextGraveOrder = await context.UnitOfWork.GameCards.GetNextGraveOrderAsync(card.PlayerGameId);
        card.Zone = CardZone.Grave;
        card.FieldIndex = null;
        card.IsFaceDown = false;
        card.DefensePosition = false;
        card.DeckOrder = nextGraveOrder;
        context.UnitOfWork.GameCards.Update(card);
    }

    private static async Task<bool> HasAvailableAttackerAsync(
        GameCommandContext context,
        Guid playerGameId,
        IReadOnlyCollection<Guid> additionalAttackedCardIds,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var player = await context.UnitOfWork.PlayerGames.GetByIdAsync(playerGameId);
        if (player is null || player.LifePoints <= 0)
            return false;

        var cards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);
        var attackedCardIds = await context.UnitOfWork.Attacks.GetAttackerCardIdsByTurnAsync(context.CurrentTurn.Id);
        var blockedCardIds = attackedCardIds.Concat(additionalAttackedCardIds).ToHashSet();

        return cards.Any(card =>
            card.PlayerGameId == playerGameId &&
            card.Zone == CardZone.Field &&
            card.FieldIndex is >= 0 and <= 4 &&
            !card.IsFaceDown &&
            !card.DefensePosition &&
            card.Card is MonsterCard &&
            !blockedCardIds.Contains(card.Id));
    }
}
