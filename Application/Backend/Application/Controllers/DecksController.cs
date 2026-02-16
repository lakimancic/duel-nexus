using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.DTOs.Decks;
using Backend.Application.Services.Interfaces;
using Backend.Domain;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers;

[Authorize]
[ApiController]
[Route("decks")]
public class DecksController(IDeckService deckService, IUserService userService) : ControllerBase
{
    private readonly IDeckService _deckService = deckService;
    private readonly IUserService _userService = userService;

    [HttpGet("limits")]
    public IActionResult GetDeckLimits()
    {
        return Ok(new
        {
            maxDecks = GameConstants.MaxDecksPerUser,
            maxDeckSize = GameConstants.MaxDeckSize
        });
    }

    [HttpGet("me/complete")]
    public async Task<IActionResult> GetMyCompleteDecks()
    {
        var userId = GetUserId();
        var decks = await _deckService.GetCompleteDecksByUserId(userId);
        return Ok(decks);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyDecks()
    {
        var userId = GetUserId();
        var decks = await _deckService.GetDecksByUserId(userId);
        return Ok(decks);
    }

    [HttpPost("me")]
    public async Task<IActionResult> CreateMyDeck([FromBody] CreateMyDeckDto dto)
    {
        var userId = GetUserId();
        var deck = await _deckService.CreateDeckForUser(userId, dto.Name);
        return Ok(deck);
    }

    [HttpGet("me/cards")]
    public async Task<IActionResult> GetMyCards()
    {
        var userId = GetUserId();
        var cards = await _userService.GetAllPlayerCards(userId);
        return Ok(cards);
    }

    [HttpGet("me/{deckId}/cards")]
    public async Task<IActionResult> GetMyDeckCards(Guid deckId)
    {
        var userId = GetUserId();
        var cards = await _deckService.GetDeckCardsByUser(userId, deckId);
        return Ok(cards);
    }

    [HttpPost("me/{deckId}/cards/add")]
    public async Task<IActionResult> AddCardToMyDeck(Guid deckId, [FromBody] EditDeckCardQuantityDto dto)
    {
        var userId = GetUserId();
        await _deckService.AddCardForUser(userId, deckId, dto.CardId, dto.Quantity);
        return Ok(new { message = "Card added to deck." });
    }

    [HttpPost("me/{deckId}/cards/remove")]
    public async Task<IActionResult> RemoveCardFromMyDeck(Guid deckId, [FromBody] EditDeckCardQuantityDto dto)
    {
        var userId = GetUserId();
        await _deckService.RemoveCardForUser(userId, deckId, dto.CardId, dto.Quantity);
        return Ok(new { message = "Card removed from deck." });
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
