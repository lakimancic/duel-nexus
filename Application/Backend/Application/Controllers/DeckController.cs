using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Decks;

namespace Backend.Application.Controllers;

[ApiController]
[Route("decks")]
public class DeckController(IDeckService deckService) : ControllerBase
{
    private readonly IDeckService _deckService = deckService;

    [HttpPost("")]
    public async Task<IActionResult> CreateDeck([FromBody] DeckDto deck)
    {
        var createdDeck = await _deckService.CreateDeck(deck);
        return Ok(createdDeck);
    }

    [HttpPost("{deckId}/cards")]
    public async Task<IActionResult> AddCards(Guid deckId, [FromBody]List<DeckCardDto> cards)
    {
        await _deckService.AddCards(deckId, cards);
        return Ok();
    }

    [HttpDelete("{deckId}/cards")]
    public async Task<IActionResult> RemoveCards(Guid deckId, [FromBody]List<Guid> cardIds)
    {
        if(cardIds.Count == 0)
            return BadRequest(new { error = "No cards to remove" });

        await _deckService.RemoveCards(cardIds);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeckById(Guid id)
    {
        var deck = await _deckService.GetDeckById(id);
        if (deck == null) return BadRequest(new { error = "Deck with id " + id + " not found" });
        return Ok(deck);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllDecks()
    {
        var decks = await _deckService.GetAllDecks();
        return Ok(decks);
    }


}
