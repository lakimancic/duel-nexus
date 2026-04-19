namespace Backend.Domain.Effects;

using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

public static class ProtectionEffectHelper
{
    public static async Task<HashSet<Guid>> GetProtectedCardIdsAsync(
        IUnitOfWork unitOfWork,
        Guid gameId,
        Turn currentTurn,
        CancellationToken cancellationToken = default)
    {
        var protectionActivations = await unitOfWork.Context.EffectActivations
            .Where(activation =>
                activation.Resolved &&
                activation.Effect.Type == Data.Enums.EffectType.ProtectCards &&
                activation.Turn.GameId == gameId)
            .Select(activation => new
            {
                activation.Id,
                ActivationTurnNumber = activation.Turn.TurnNumber,
                activation.Effect.Turns,
            })
            .ToListAsync(cancellationToken);

        var activeProtectionActivations = protectionActivations
            .Where(activation =>
            {
                var turns = activation.Turns.GetValueOrDefault(1);
                if (turns <= 0)
                    turns = 1;

                var lastProtectedTurn = activation.ActivationTurnNumber + turns - 1;
                return currentTurn.TurnNumber <= lastProtectedTurn;
            })
            .Select(activation => activation.Id)
            .ToList();

        if (activeProtectionActivations.Count == 0)
            return [];

        return await unitOfWork.Context.EffectTargets
            .Where(target =>
                target.TargetCardId.HasValue &&
                activeProtectionActivations.Contains(target.ActivationId))
            .Select(target => target.TargetCardId!.Value)
            .ToHashSetAsync(cancellationToken);
    }
}
