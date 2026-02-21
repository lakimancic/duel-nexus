using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.DTOs.Games;
using Backend.Application.Services.Interfaces;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Application.Controllers;

[Authorize]
[ApiController]
[Route("games")]
public class GamesController(
    IGameService gameService,
    IHubContext<GameHub> hubContext
) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private readonly IHubContext<GameHub> _hubContext = hubContext;

    [HttpGet("{id}/state")]
    public async Task<IActionResult> GetGameState(Guid id)
    {
        var userId = GetUserId();
        var state = await _gameService.GetGameState(id, userId);
        return Ok(state);
    }

    [HttpPost("{id}/actions/draw")]
    public async Task<IActionResult> DrawCard(Guid id)
    {
        var userId = GetUserId();
        var result = await _gameService.DrawCard(id, userId);

        var drawEvent = new PlayerDrewCardEventDto
        {
            GameId = result.GameId,
            PlayerGameId = result.PlayerGameId,
            TurnId = result.TurnId,
            DrawsInTurn = result.DrawsInTurn,
            TurnEnded = result.TurnEnded,
            NextActivePlayerId = result.NextActivePlayerId,
        };

        await _hubContext.Clients.Group(GameHub.GetGameGroupName(id))
            .SendAsync("game:player:drew", drawEvent);
        await _hubContext.Clients.Group(GameHub.GetGameRoomGroupName(result.RoomId))
            .SendAsync("game:player:drew", drawEvent);

        return Ok(result);
    }

    private Guid GetUserId()
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var nameIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userIdString = subClaim ?? nameIdClaim;
        if (string.IsNullOrWhiteSpace(userIdString))
            throw new BadRequestException("User ID not found in token.");

        return Guid.Parse(userIdString);
    }
}
