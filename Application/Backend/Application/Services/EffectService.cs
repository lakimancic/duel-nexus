using AutoMapper;
using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Effects;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;

namespace Backend.Application.Services;

public class EffectService(IUnitOfWork unitOfWork, IMapper mapper) : IEffectService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<EffectDto?> GetEffectById(Guid id)
    {
        var effect = await _unitOfWork.Effects.GetByIdAsync(id);
        if(effect == null) return null;
        return _mapper.Map<Effect,EffectDto>(effect);
    }

    public async Task<EffectDto> CreateEffect(CreateEffectDto effect)
    {
        var effectEntity = _mapper.Map<CreateEffectDto, Effect>(effect);
        await _unitOfWork.Effects.AddAsync(effectEntity);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Effect, EffectDto>(effectEntity);
    }

    public async Task DeleteEffect(Guid id)
    {
        var effectEntity = await _unitOfWork.Effects.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Effect not found");
        _unitOfWork.Effects.Delete(effectEntity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PagedResult<EffectDto>> GetEffectsAsync(int page, int pageSize)
    {
        var effects = await _unitOfWork.Effects.GetPagedAsync(page, pageSize, q => q.OrderBy(d => d.Id));
        return _mapper.Map<PagedResult<EffectDto>>(effects);
    }
}
