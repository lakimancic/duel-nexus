using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Utils.WebApi;

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

    [Authorize]
    [HttpGet("private/conversations")]
    public async Task<IActionResult> GetPrivateConversations()
    {
        var userId = GetUserId();
        var conversations = await _chatService.GetPrivateConversationsAsync(userId);
        return Ok(conversations);
    }

    [Authorize]
    [HttpGet("private/{otherUserId}")]
    public async Task<IActionResult> GetPrivateMessages(Guid otherUserId, int page = 1, int pageSize = 10)
    {
        var userId = GetUserId();
        pageSize = Math.Min(pageSize, MaxPageSize);
        var messages = await _chatService.GetPrivateMessagesAsync(userId, otherUserId, page, pageSize);
        return Ok(messages);
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
