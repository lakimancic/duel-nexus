namespace Backend.Domain.Effects;

using Backend.Data.Enums;
using Backend.Utils.WebApi;

public sealed class EffectStrategyFactory
{
    private readonly IReadOnlyDictionary<EffectType, IEffectStrategy> _strategies;

    public EffectStrategyFactory()
    {
        _strategies = new Dictionary<EffectType, IEffectStrategy>
        {
            [EffectType.DrawCards] = new DrawCardsEffectStrategy(),
            [EffectType.DestroyCards] = new DestroyCardsEffectStrategy(),
            [EffectType.AttackLifePoints] = new AttackLifePointsEffectStrategy(),
            [EffectType.ProtectCards] = new ProtectCardsEffectStrategy(),
            [EffectType.RestoreCards] = new RestoreCardsEffectStrategy(),
            [EffectType.HealLifePoints] = new HealLifePointsEffectStrategy(),
        };
    }

    public IEffectStrategy GetStrategy(EffectType effectType)
    {
        if (!_strategies.TryGetValue(effectType, out var strategy))
            throw new BadRequestException($"Unsupported effect type '{effectType}'.");

        return strategy;
    }
}
