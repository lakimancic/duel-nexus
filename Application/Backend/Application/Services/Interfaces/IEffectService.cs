using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Effects;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IEffectService
{
    public Task<PagedResult<EffectDto>> GetEffectsAsync(int page, int pageSize);
    public Task<EffectDto?> GetEffectById(Guid id);
    public Task<EffectDto> CreateEffect(CreateEffectDto effect);
    public Task DeleteEffect(Guid id);
}
