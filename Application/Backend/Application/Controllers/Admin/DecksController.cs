using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Decks;

namespace Backend.Application.Controllers.Admin;

[ApiController]
[Route("admin/decks")]
public class DecksController(IDeckService deckService) : ControllerBase
{
    private readonly IDeckService _deckService = deckService;
    private const int MaxPageSize = 30;

    [HttpPost]
    public async Task<IActionResult> CreateDeck([FromBody] CreateDeckDto deck)
    {
        var createdDeck = await _deckService.CreateDeck(deck);
        return Ok(createdDeck);
    }

    [HttpGet]
    public async Task<IActionResult> GetDecks(int page = 1, int pageSize = 10, string? search = null)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var cards = await _deckService.GetDecks(page, pageSize, search);
        return Ok(cards);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeck(Guid id)
    {
        var deck = await _deckService.GetDeckWithUser(id);
        if (deck == null)
            return NotFound(new { error = "Deck not found"});
        return Ok(deck);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeck(Guid id, [FromBody] EditDeckDto deck)
    {
        var updatedDeck = await _deckService.EditDeck(id, deck);
        return Ok(updatedDeck);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeck(Guid id)
    {
        await _deckService.DeleteDeck(id);
        return Ok(new { message = "Deck deleted successfully" });
    }

    [HttpGet("{id}/cards")]
    public async Task<IActionResult> GetDeckCards(Guid id)
    {
        var cards = await _deckService.GetDeckCards(id);
        return Ok(cards);
    }

    [HttpPost("{id}/cards")]
    public async Task<IActionResult> AddCards(Guid id, [FromBody] List<InsertDeckCardDto> cards)
    {
        await _deckService.AddCards(id, cards);
        return Ok(new { message = "Cards added successfully to Deck" });
    }

    [HttpPost("{id}/remove-cards")]
    public async Task<IActionResult> RemoveCards(Guid id, [FromBody] List<Guid> cardIds)
    {
        await _deckService.RemoveCards(id, cardIds);
        return Ok(new { message = "Cards deleted successfully from Deck" });
    }
}
