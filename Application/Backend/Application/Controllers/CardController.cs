using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;

namespace Backend.Api.Controllers;

[ApiController]
[Route("cards")]
public class CardController(ICardService cardService) : ControllerBase
{
    private readonly ICardService _cardService = cardService;


    [HttpGet("{id}")]
    public async Task<IActionResult> GetCardById(Guid id)
    {
        var card = await _cardService.GetCardById(id);
        if (card == null) return BadRequest(new { error = "Card with id " + id + " not found" });
        return Ok(card);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAllCards()
    {
        var cards = await _cardService.GetAllCards();
        return Ok(cards);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateCard([FromBody] CardDto card)
    {
        if(card.Type == CardType.Monster)
        {
            card.Attack = 0;
            card.Defense = 0;
            card.Level = 0;
        }

        var createdCard = await _cardService.CreateCard(card);
        return Ok(createdCard);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCard(Guid id, [FromBody] CardDto card)
    {
        if(card.Type != CardType.Monster)
        {
            card.Attack = 0;
            card.Defense = 0;
            card.Level = 0;
        }

        var updatedCard = await _cardService.EditCard(id, card);
        return Ok(updatedCard);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCard(Guid id)
    {
        await _cardService.DeleteCard(id);
        return Ok(new { message = "Card deleted successfully" });
    }
}