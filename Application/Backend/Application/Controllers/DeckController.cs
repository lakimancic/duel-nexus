using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Decks;

namespace Backend.Application.Controllers;

[ApiController]
[Route("admin/decks")]
public class DeckController(IDeckService deckService) : ControllerBase
{
    private readonly IDeckService _deckService = deckService;
    private const int MaxPageSize = 30;

    [HttpPost]
    public async Task<IActionResult> CreateDeck([FromBody] DeckDto deck)
    {
        var createdDeck = await _deckService.CreateDeck(deck);
        return Ok(createdDeck);
    }

    [HttpPost("{deckId}/cards")]
    public async Task<IActionResult> AddCards(Guid deckId, [FromBody]List<InsertDeckCardDto> cards)
    {
        await _deckService.AddCards(deckId, cards);
        return Ok();
    }

    [HttpDelete("{deckId}/cards")]
    public async Task<IActionResult> RemoveCards(Guid deckId, [FromBody]List<Guid> cardIds)
    {
        if(cardIds.Count == 0)
            return BadRequest(new { error = "No cards to remove" });

        await _deckService.RemoveCards(deckId, cardIds);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeckById(Guid id)
    {
        var deck = await _deckService.GetDeckById(id);
        if (deck == null) return BadRequest(new { error = "Deck with id " + id + " not found" });
        return Ok(deck);
    }

    [HttpGet]
    public async Task<IActionResult> GetCards(int page = 1, int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var cards = await _deckService.GetDecksAsync(page, pageSize);
        return Ok(cards);
    }
}
