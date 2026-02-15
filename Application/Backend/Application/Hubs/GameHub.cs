using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public partial class GameHub(IChatService chatService, IGameRoomService gameRoomService) : Hub
{
    protected readonly IChatService Chat = chatService;
    protected readonly IGameRoomService Rooms = gameRoomService;

    private Guid GetUserId()
    {
        var subClaim = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var nameIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userIdString = subClaim ?? nameIdClaim;
        if (string.IsNullOrEmpty(userIdString))
            throw new HubException("User ID not found in token");

        return Guid.Parse(userIdString);
    }
}
