using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Effects;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IEffectService
{
    Task<PagedResult<EffectDto>> GetEffectsAsync(int page, int pageSize);
    Task<EffectDto?> GetEffectById(Guid id);
    Task<EffectDto> CreateEffect(CreateEffectDto effect);
    Task DeleteEffect(Guid id);
    Task<EffectDto> EditEffect(Guid id, CreateEffectDto effect);
}
