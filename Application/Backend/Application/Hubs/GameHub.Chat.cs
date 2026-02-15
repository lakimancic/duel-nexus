using Backend.Application.DTOs.Chat;
using Microsoft.AspNetCore.SignalR;

public partial class GameHub
{
    [HubMethodName("chat:global:send")]
    public async Task SendGlobalMessage(string content)
    {
        var userId = GetUserId();
        var message = await Chat.SendMessageAsync(
            new SendMessageDto { SenderId = userId, Content = content });

        await Clients.All.SendAsync("chat:global:recv", message);
    }

    [HubMethodName("chat:private:send")]
    public async Task SendPrivateMessage(Guid recvId, string content)
    {
        var userId = GetUserId();
        var message = await Chat.SendPrivateMessageAsync(new PrivateMessageDto
        {
            SenderId = userId,
            ReceiverId = recvId,
            Content = content
        });

        await Clients.User(recvId.ToString()).SendAsync("chat:private:recv", message);
        await Clients.Caller.SendAsync("chat:private:recv", message);
    }

    [HubMethodName("chat:game-room:send")]
    public async Task SendGameRoomMessage(Guid gameRoomId, string content)
    {
        var userId = GetUserId();
        var message = await Chat.SendGameRoomMessageAsync(new GameRoomMessageDto
        {
            SenderId = userId,
            GameRoomId = gameRoomId,
            Content = content
        });

        await Clients.Group(gameRoomId.ToString()).SendAsync("chat:game-room:recv", message);
    }
}
