using Microsoft.AspNetCore.SignalR;

public partial class GameHub
{
    [HubMethodName("game-room:join")]
    public async Task JoinGameRoom(Guid gameRoomId)
    {
        var userId = GetUserId();
        if (!await Rooms.UserExistsInRoom(gameRoomId, userId))
            return;
        await Groups.AddToGroupAsync(Context.ConnectionId, gameRoomId.ToString());
    }

    [HubMethodName("game-room:leave")]
    public async Task LeaveGameRoom(Guid gameRoomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameRoomId.ToString());
    }
}
