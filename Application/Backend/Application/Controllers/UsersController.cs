using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.Services.Interfaces;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("scoreboard")]
    public async Task<IActionResult> GetScoreboard(int limit = 50)
    {
        var scoreboard = await _userService.GetScoreboardAsync(limit);
        return Ok(scoreboard);
    }

    [HttpGet("me/profile-stats")]
    public async Task<IActionResult> GetMyProfileStats(int gamesLimit = 20)
    {
        var userId = GetUserId();
        var stats = await _userService.GetUserProfileStatsAsync(userId, gamesLimit);
        return Ok(stats);
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
