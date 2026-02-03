using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Games;

[ApiController]
[Route("admin/games")]
public class GameController(IGameService gameService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;

    [HttpPut("{gameId}/end-phase")]
    public async Task<IActionResult> EndGamePhase(Guid gameId)
    {
        await _gameService.EndGamePhaseAsync(gameId);
        return Ok();
    }

    [HttpPut("{gameId}/end-turn")]
    public async Task<IActionResult> EndGameTurn(Guid gameId)
    {
        await _gameService.EndGameTurnAsync(gameId);
        return Ok();
    }

    [HttpGet("{gameId}/players/{playerId}/draw-card")]
    public async Task<IActionResult> DrawCard(Guid gameId, Guid playerId)
    {
        var card = await _gameService.DrawCardAsync(gameId, playerId);
        return Ok(card);
    }

    [HttpPost("{gameId}/players/{playerId}/play-card")]
    public async Task<IActionResult> PlayCard(Guid gameId, Guid playerId, [FromBody] PlayCardDto playCard)
    {
        await _gameService.PlayCardAsync(gameId, playerId, playCard);
        return Ok();
    }

    [HttpPost("{gameId}/players/{playerId}/activate-effect")]
    public async Task<IActionResult> ActivateEffect(Guid gameId, Guid playerId, [FromBody] ActivateEffectDto effectDto)
    {
        await _gameService.ActivateEffectAsync(gameId, playerId, effectDto);
        return Ok();
    }

    [HttpPost("{gameId}/players/{playerId}/attack")]
    public async Task<IActionResult> ActivateEffect(Guid gameId, Guid playerId, [FromBody] AttackActionDto attack)
    {
        await _gameService.AttackActionAsync(gameId, playerId, attack);
        return Ok();
    }
}
