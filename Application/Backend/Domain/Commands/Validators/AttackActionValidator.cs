namespace Backend.Domain.Commands.Validators;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed class AttackActionValidator : IGameCommandValidator<AttackActionCommand, BattleAttackResult>
{
    public async Task ValidateAsync(AttackActionCommand command, GameCommandContext context, CancellationToken cancellationToken = default)
    {
        if (context.Game.FinishedAt is not null)
            throw new BadRequestException("Game is already finished.");

        if (context.Actor.LifePoints <= 0)
            throw new BadRequestException("You already lost this game.");

        if (context.CurrentTurn.Phase != TurnPhase.Battle)
            throw new BadRequestException("Attack is allowed only in Battle phase.");

        if (context.CurrentTurn.ActivePlayerId != context.Actor.Id)
            throw new BadRequestException("Only active player can attack in Battle phase.");

        var hasDefenderCard = command.DefenderCardId.HasValue;
        var hasDefenderPlayer = command.DefenderPlayerGameId.HasValue;
        if (hasDefenderCard == hasDefenderPlayer)
            throw new BadRequestException("Attack must target either one card or one player.");

        var attacker = await context.UnitOfWork.GameCards.GetByWithCardById(command.AttackerCardId)
            ?? throw new ObjectNotFoundException("Attacker card not found.");

        if (attacker.PlayerGameId != context.Actor.Id)
            throw new BadRequestException("You can attack only with your own card.");

        if (attacker.Zone != CardZone.Field || attacker.FieldIndex is null)
            throw new BadRequestException("Attacker card must be on field.");

        if (attacker.FieldIndex > 4)
            throw new BadRequestException("Only monster field cards can attack.");

        if (attacker.IsFaceDown)
            throw new BadRequestException("Face-down monsters cannot attack.");

        if (attacker.DefensePosition)
            throw new BadRequestException("Defense position monsters cannot attack.");

        if (attacker.Card is not MonsterCard)
            throw new BadRequestException("Only monster cards can attack.");

        var alreadyAttacked = await context.UnitOfWork.Attacks.HasAttackForTurnAndAttackerAsync(
            context.CurrentTurn.Id,
            attacker.Id);

        if (alreadyAttacked)
            throw new BadRequestException("This monster already attacked in current Battle phase.");

        if (hasDefenderCard)
        {
            var defender = await context.UnitOfWork.GameCards.GetByWithCardById(command.DefenderCardId!.Value)
                ?? throw new ObjectNotFoundException("Defender card not found.");

            if (defender.PlayerGameId == context.Actor.Id)
                throw new BadRequestException("You cannot attack your own card.");

            if (defender.Zone != CardZone.Field || defender.FieldIndex is null)
                throw new BadRequestException("Defender card must be on field.");

            if (defender.FieldIndex > 4)
                throw new BadRequestException("Only monster field cards can be attacked.");

            if (defender.Card is not MonsterCard)
                throw new BadRequestException("Only monster cards can be attacked.");

            var defenderPlayer = await context.UnitOfWork.PlayerGames.GetByIdAsync(defender.PlayerGameId)
                ?? throw new ObjectNotFoundException("Defender player not found.");
            if (defenderPlayer.LifePoints <= 0)
                throw new BadRequestException("Selected player already lost.");
        }
        else
        {
            var defenderPlayer = await context.UnitOfWork.PlayerGames.GetByIdAsync(command.DefenderPlayerGameId!.Value)
                ?? throw new ObjectNotFoundException("Defender player not found.");

            if (defenderPlayer.GameId != context.Game.Id)
                throw new BadRequestException("Defender player is not part of this game.");

            if (defenderPlayer.Id == context.Actor.Id)
                throw new BadRequestException("You cannot attack yourself.");

            if (defenderPlayer.LifePoints <= 0)
                throw new BadRequestException("Selected player already lost.");

            var cards = await context.UnitOfWork.GameCards.GetByGameIdWithCardAsync(context.Game.Id);
            var defenderHasMonstersOnField = cards.Any(card =>
                card.PlayerGameId == defenderPlayer.Id &&
                card.Zone == CardZone.Field &&
                card.FieldIndex is >= 0 and <= 4 &&
                card.Card is MonsterCard);

            if (defenderHasMonstersOnField)
                throw new BadRequestException("Direct attack is allowed only when defender has no monsters on field.");
        }
    }
}
