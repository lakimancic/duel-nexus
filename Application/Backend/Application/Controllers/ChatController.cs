using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;

namespace Backend.Application.Controllers;

[ApiController]
[Route("chat")]
public class ChatController(IChatService chatService) : ControllerBase
{
    private readonly IChatService _chatService = chatService;
    private const int MaxPageSize = 20;

    [HttpGet]
    public async Task<IActionResult> GetMessages(int page = 1, int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var messages = await _chatService.GetGlobalMessagesAsync(page, pageSize);
        return Ok(messages);
    }

    [HttpGet("game-room/{id}")]
    public async Task<IActionResult> GetGameRoomMessages(Guid id, int page = 1, int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var messages = await _chatService.GetGameRoomMessagesAsync(id, page, pageSize);
        return Ok(messages);
    }
}
