using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Decks;

namespace Backend.Application.Controllers;

[ApiController]
[Route("effects")]
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

    [HttpGet()]
    public async Task<IActionResult> GetAllEffects()
    {
        var effects = await _effectService.GetAllEffects();
        return Ok(effects);
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEffect(Guid id)
    {
        await _effectService.DeleteEffect(id);
        return Ok();
    }
}
