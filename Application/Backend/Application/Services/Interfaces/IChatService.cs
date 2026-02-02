using Backend.Application.DTOs.Chat;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IChatService
{
    Task<PagedResult<ChatMessageDto>> GetPrivateMessagesAsync(Guid userId1, Guid userId2, int page, int pageSize);
    Task<PagedResult<ChatMessageDto>> GetGameRoomMessagesAsync(Guid gameRoomId, int page, int pageSize);
    Task<PagedResult<ChatMessageDto>> GetGlobalMessagesAsync(int page, int pageSize);
    Task SendMessageAsync(SendMessageDto message);
    Task SendGameRoomMessageAsync(GameRoomMessageDto message);
    Task SendPrivateMessageAsync(PrivateMessageDto message);
    Task DeleteMessageAsync(Guid messageId);
}
