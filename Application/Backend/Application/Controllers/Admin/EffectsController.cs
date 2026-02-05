using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Effects;
using Backend.Data.Enums;
using Backend.Utils.Data;

namespace Backend.Application.Controllers.Admin;

[ApiController]
[Route("admin/effects")]
public class EffectsController(IEffectService effectService) : ControllerBase
{
    private readonly IEffectService _effectService = effectService;

    [HttpPost]
    public async Task<IActionResult> CreateEffect([FromBody] CreateEffectDto effect)
    {
        var createdEffect = await _effectService.CreateEffect(effect);
        return Ok(createdEffect);
    }

    [HttpGet]
    public async Task<IActionResult> GetEffects(int page = 1, int pageSize = 10)
    {
        var effects = await _effectService.GetEffectsAsync(page, pageSize);
        return Ok(effects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEffectById(Guid id)
    {
        var effect = await _effectService.GetEffectById(id);
        if (effect == null)
            return NotFound(new { error = "Effect not found" });
        return Ok(effect);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEffect(Guid id, [FromBody] CreateEffectDto effect)
    {
        var updatedEffect = await _effectService.EditEffect(id, effect);
        return Ok(updatedEffect);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEffect(Guid id)
    {
        await _effectService.DeleteEffect(id);
        return Ok();
    }

    [HttpGet("types")]
    public IActionResult GetTypes()
    {
        return Ok(EnumExtensions.GetNameValues<EffectType>());
    }
}
