using Backend.Application.DTOs.Chat;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IChatService
{
    Task<PagedResult<ChatMessageDto>> GetPrivateMessagesAsync(Guid userId1, Guid userId2, int page, int pageSize);
    Task<List<PrivateConversationDto>> GetPrivateConversationsAsync(Guid userId);
    Task<PagedResult<ChatMessageDto>> GetGameRoomMessagesAsync(Guid gameRoomId, int page, int pageSize);
    Task<PagedResult<ChatMessageDto>> GetGlobalMessagesAsync(int page, int pageSize);
    Task<ChatMessageDto> SendMessageAsync(SendMessageDto message);
    Task<ChatMessageDto> SendGameRoomMessageAsync(GameRoomMessageDto message);
    Task<ChatMessageDto> SendPrivateMessageAsync(PrivateMessageDto message);
    Task DeleteMessageAsync(Guid messageId);
    Task<ChatMessageDto> EditMessageAsync(Guid messageId, EditMessageDto message);
}
