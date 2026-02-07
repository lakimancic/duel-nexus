using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IEffectActivationRepository : IRepository<EffectActivation>
{
    Task<EffectActivation> ActivateEffectAsync(Turn turn, Effect effect, GameCard gameCard);
    Task<EffectActivation?> GetByIdWithIncludesAsync(Guid id);
}
