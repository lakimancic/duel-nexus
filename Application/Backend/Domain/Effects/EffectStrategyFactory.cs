namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Utils.WebApi;

public sealed class EffectStrategyFactory
{
    public static IEffectStrategy CreateStrategy(EffectType effectType)
    {
        return effectType switch
        {
            EffectType.DrawCards => new DrawCardsEffectStrategy(),
            EffectType.DestroyCards => new DestroyCardsEffectStrategy(),
            EffectType.AttackLifePoints => new AttackLifePointsEffectStrategy(),
            EffectType.ProtectCards => new ProtectCardsEffectStrategy(),
            EffectType.RestoreCards => new RestoreCardsEffectStrategy(),
            EffectType.HealLifePoints => new HealLifePointsEffectStrategy(),
            _ => throw new BadRequestException(
                $"Unsupported effect type '{effectType}'.")
        };
    }
}
