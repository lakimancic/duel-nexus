using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Effects;

namespace Backend.Application.Controllers;

[ApiController]
[Route("admin/effects")]
public class EffectController(IEffectService effectService) : ControllerBase
{
    private readonly IEffectService _effectService = effectService;

    [HttpPost("")]
    public async Task<IActionResult> CreateEffect([FromBody] CreateEffectDto effect)
    {
        var createdEffect = await _effectService.CreateEffect(effect);
        return Ok(createdEffect);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEffectById(Guid id)
    {
        var effect = await _effectService.GetEffectById(id);
        if (effect == null) return BadRequest(new { error = "Effect with id " + id + " not found" });
        return Ok(effect);
    }

    [HttpGet]
    public async Task<IActionResult> GetEffects(int page = 1, int pageSize = 10)
    {
        var effects = await _effectService.GetEffectsAsync(page, pageSize);
        return Ok(effects);
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEffect(Guid id)
    {
        await _effectService.DeleteEffect(id);
        return Ok();
    }
}
