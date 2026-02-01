using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Application.DTOs.Decks;

namespace Backend.Application.Controllers;

[ApiController]
[Route("admin/cards")]
public class CardController(ICardService cardService) : ControllerBase
{
    private readonly ICardService _cardService = cardService;
    private const int MaxPageSize = 50;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCardById(Guid id)
    {
        var card = await _cardService.GetCardById(id);
        if (card == null) return BadRequest(new { error = "Card with id " + id + " not found" });
        return Ok(card);
    }

    [HttpGet]
    public async Task<IActionResult> GetCards(int page = 1, int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var cards = await _cardService.GetCardsAsync(page, pageSize);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] CreateCardDto card)
    {
        var createdCard = await _cardService.CreateCard(card);
        return Ok(createdCard);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCard(Guid id, [FromBody] CreateCardDto card)
    {
        var updatedCard = await _cardService.EditCard(id, card);
        return Ok(updatedCard);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCard(Guid id)
    {
        await _cardService.DeleteCard(id);
        return Ok(new { message = $"Card with id {id} deleted successfully" });
    }
}
