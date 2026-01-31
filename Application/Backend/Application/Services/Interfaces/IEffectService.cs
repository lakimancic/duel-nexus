using Backend.Application.DTOs.Decks;

namespace Backend.Application.Services.Interfaces;

public interface IEffectService
{
    public Task<List<EffectDto>> GetAllEffects();

    public Task<EffectDto?> GetEffectById(Guid id);
    public Task<EffectDto> CreateEffect(CreateEffectDto effect);
    public Task DeleteEffect(Guid id);
}
