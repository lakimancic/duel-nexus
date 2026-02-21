using Microsoft.AspNetCore.SignalR;

public partial class GameHub
{
    [HubMethodName("game:join")]
    public async Task JoinGame(Guid gameId)
    {
        var userId = GetUserId();
        if (!await Games.UserExistsInGame(gameId, userId))
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, GetGameGroupName(gameId));
    }

    [HubMethodName("game:leave")]
    public async Task LeaveGame(Guid gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGameGroupName(gameId));
    }
}
