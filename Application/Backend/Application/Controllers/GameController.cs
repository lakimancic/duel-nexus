using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;

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
}
