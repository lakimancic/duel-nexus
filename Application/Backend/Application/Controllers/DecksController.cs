using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.Services.Interfaces;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers;

[Authorize]
[ApiController]
[Route("decks")]
public class DecksController(IDeckService deckService) : ControllerBase
{
    private readonly IDeckService _deckService = deckService;

    [HttpGet("me/complete")]
    public async Task<IActionResult> GetMyCompleteDecks()
    {
        var userId = GetUserId();
        var decks = await _deckService.GetCompleteDecksByUserId(userId);
        return Ok(decks);
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
