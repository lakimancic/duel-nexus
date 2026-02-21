using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public partial class GameHub(
    IChatService chatService,
    IGameRoomService gameRoomService,
    IGameService gameService,
    IUserService userService,
    IConnectionService connectionService
    ) : Hub
{
    protected readonly IChatService Chat = chatService;
    protected readonly IGameRoomService Rooms = gameRoomService;
    protected readonly IGameService Games = gameService;
    protected readonly IUserService Users = userService;
    protected readonly IConnectionService Connections = connectionService;
    public static string GetGameRoomGroupName(Guid gameRoomId) => $"game-room:{gameRoomId}";
    public static string GetGameGroupName(Guid gameId) => $"game:{gameId}";

    private Guid GetUserId()
    {
        var subClaim = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var nameIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userIdString = subClaim ?? nameIdClaim;
        if (string.IsNullOrEmpty(userIdString))
            throw new HubException("User ID not found in token");

        return Guid.Parse(userIdString);
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = GetUserId();
            var addedId = Connections.AddOnlineUser(userId, Context.ConnectionId);

            if (addedId != null)
            {
                var user = await Users.GetShortUserById(addedId.Value);
                if (user != null)
                    await Clients.All.SendAsync("users:connected", user);
            }
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = GetUserId();
            var removedId = Connections.RemoveOnlineUser(userId, Context.ConnectionId);

            if (removedId != null)
            {
                await Clients.All.SendAsync("users:disconnected", removedId.Value);
            }
        }
        catch(Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
