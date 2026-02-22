using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.Services.Interfaces;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers;

[Authorize]
[ApiController]
[Route("games")]
public class GamesController(
    IGameService gameService
) : ControllerBase
{
    private readonly IGameService _gameService = gameService;

    [HttpGet("{id}/state")]
    public async Task<IActionResult> GetGameState(Guid id)
    {
        var userId = GetUserId();
        var state = await _gameService.GetGameState(id, userId);
        return Ok(state);
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
