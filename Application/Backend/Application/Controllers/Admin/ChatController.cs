using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Chat;

namespace Backend.Application.Controllers.Admin;

[ApiController]
[Route("admin/messages")]
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

    [HttpGet("game-room/{gameRoomId}")]
    public async Task<IActionResult> GetGameRoomMessages(Guid gameRoomId, int page = 1, int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var messages = await _chatService.GetGameRoomMessagesAsync(gameRoomId, page, pageSize);
        return Ok(messages);
    }

    [HttpGet("private/{userId1}/{userId2}")]
    public async Task<IActionResult> GetPrivateMessages(Guid userId1, Guid userId2, int page = 1, int pageSize = 10)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var messages = await _chatService.GetPrivateMessagesAsync(userId1, userId2, page, pageSize);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto message)
    {
        await _chatService.SendMessageAsync(message);
        return Ok();
    }

    [HttpPost("game-room")]
    public async Task<IActionResult> SendGameRoomMessage([FromBody] GameRoomMessageDto message)
    {
        await _chatService.SendGameRoomMessageAsync(message);
        return Ok();
    }

    [HttpPost("private")]
    public async Task<IActionResult> SendPrivateMessage([FromBody] PrivateMessageDto message)
    {
        await _chatService.SendPrivateMessageAsync(message);
        return Ok();
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        await _chatService.DeleteMessageAsync(messageId);
        return Ok();
    }
}
