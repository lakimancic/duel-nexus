using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Player;

namespace Backend.Application.Controllers;

[ApiController]
[Route("player-card")]
public class PlayerCardController(IPlayerCardService playerCardService) : ControllerBase
{
    private readonly IPlayerCardService _playerCardService = playerCardService;

    [HttpGet]
    public async Task<IActionResult> GetAllPlayerCards()
    {
        var playerCards = await _playerCardService.GetAllPlayerCards();
        return Ok(playerCards);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerCardById(Guid id)
    {
        var playerCard = await _playerCardService.GetPlayerCardById(id);
        if (playerCard == null)
        {
            return NotFound();
        }
        return Ok(playerCard);
    }

    [HttpGet("deck/{deckId}")]
    public async Task<IActionResult> GetPlayerCardsByDeckId(Guid deckId)
    {
        var playerCards = await _playerCardService.GetPlayerCardsByDeckId(deckId);
        return Ok(playerCards);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlayerCard([FromBody] CreatePlayerCardDto playerCard)
    {
        var createdPlayerCard = await _playerCardService.CreatePlayerCard(playerCard);
        return CreatedAtAction(nameof(GetPlayerCardById), new { id = createdPlayerCard.Id }, createdPlayerCard);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayerCard(Guid id)
    {
        await _playerCardService.DeletePlayerCard(id);
        return Ok(new { message = "PlayerCard deleted successfully" });
    }
}
