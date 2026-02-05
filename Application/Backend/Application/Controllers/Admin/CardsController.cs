using Backend.Application.DTOs.Cards;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Utils.Data;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace Backend.Application.Controllers.Admin;

[ApiController]
[Route("admin/cards")]
public class CardController(ICardService cardService, IConfiguration configuration) : ControllerBase
{
    private readonly ICardService _cardService = cardService;
    private readonly string _baseDir = configuration["Storage:BaseDir"]
        ?? throw new InvalidOperationException("Storage:BaseDir not configured");
    private const int MaxPageSize = 50;
    private const long MaxFileSize = 2 * 1024 * 1024;

    [HttpGet]
    public async Task<IActionResult> GetCards(int page = 1, int pageSize = 10, string? search = null)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var cards = await _cardService.GetCards(page, pageSize, search);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] CreateCardDto card)
    {
        var createdCard = await _cardService.CreateCard(card);
        return Ok(createdCard);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCard(Guid id)
    {
        var card = await _cardService.GetCardById(id);
        if (card == null)
            return NotFound(new { error = "Card not found"});
        return Ok(card);
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
        return Ok(new { message = "Card deleted successfully" });
    }

    [HttpGet("types")]
    public IActionResult GetTypes()
    {
        return Ok(EnumExtensions.GetNameValues<CardType>());
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded." });

        if (file.Length > MaxFileSize)
            return BadRequest(new { error = "File too large (max 2MB)." });

        await using var stream = file.OpenReadStream();
        using var image = await Image.LoadAsync(stream);

        if (image.Width != image.Height)
            return BadRequest(new { error = "File must be square." });

        var fileName = $"{Guid.NewGuid()}.png";
        var uploadDir = Path.Combine(_baseDir, "uploads");
        var fullPath = Path.Combine(uploadDir, fileName);

        Directory.CreateDirectory(uploadDir);

        await image.SaveAsPngAsync(fullPath);

        return Ok(new { fileName });
    }

    [HttpGet("image/{fileName}")]
    public async Task<IActionResult> GetImage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return BadRequest();


        fileName = Path.GetFileName(fileName);

        var fullPath = Path.Combine(_baseDir, "uploads", fileName);

        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        var contentType = "image/png";
        Response.Headers.CacheControl = "public,max-age=31536000,immutable";

        return PhysicalFile(fullPath, contentType, enableRangeProcessing: true);
    }
}
