using AutoMapper;
using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Users;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;
using Backend.Utils.WebApi;
using Microsoft.EntityFrameworkCore;

namespace Backend.Application.Services;

public class ChatService(IUnitOfWork unitOfWork, IMapper mapper) : IChatService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId)
            ?? throw new ObjectNotFoundException($"Message not found");
        _unitOfWork.Messages.Delete(message);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<ChatMessageDto> EditMessageAsync(Guid messageId, EditMessageDto message)
    {
        var existingMessage = await _unitOfWork.Messages.GetWithSenderById(messageId)
            ?? throw new ObjectNotFoundException($"Message not found");

        existingMessage.Content = message.Content;
        _unitOfWork.Messages.Update(existingMessage);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<ChatMessageDto>(existingMessage);
    }

    public async Task<PagedResult<ChatMessageDto>> GetGameRoomMessagesAsync(Guid gameRoomId, int page, int pageSize)
    {
        var messages = await _unitOfWork.Messages.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(m => m.SentAt), msg => msg.GameRoomId == gameRoomId, "Sender");
        return _mapper.Map<PagedResult<ChatMessageDto>>(messages);
    }

    public async Task<PagedResult<ChatMessageDto>> GetGlobalMessagesAsync(int page, int pageSize)
    {
        var messages = await _unitOfWork.Messages.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(m => m.SentAt),
            msg => msg.GameRoomId == null && msg.ReceiverId == null, includeProperties: "Sender"
        );
        return _mapper.Map<PagedResult<ChatMessageDto>>(messages);
    }

    public async Task<PagedResult<ChatMessageDto>> GetPrivateMessagesAsync(Guid userId1, Guid userId2, int page, int pageSize)
    {
        var messages = await _unitOfWork.Messages.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(m => m.SentAt),
            msg =>
                (
                    (msg.SenderId == userId1 && msg.ReceiverId == userId2) ||
                    (msg.SenderId == userId2 && msg.ReceiverId == userId1)
                ) && msg.Type == Data.Enums.MessageType.Private,
            "Sender"
        );
        return _mapper.Map<PagedResult<ChatMessageDto>>(messages);
    }

    public async Task<List<PrivateConversationDto>> GetPrivateConversationsAsync(Guid userId)
    {
        var privateMessages = await _unitOfWork.Context.Messages
            .Where(msg =>
                msg.Type == Data.Enums.MessageType.Private &&
                (msg.SenderId == userId || msg.ReceiverId == userId)
            )
            .OrderByDescending(msg => msg.SentAt)
            .ToListAsync();

        if (privateMessages.Count == 0)
            return [];

        var latestMessagesByUser = new Dictionary<Guid, ChatMessage>();
        foreach (var message in privateMessages)
        {
            var targetUserId = message.SenderId == userId ? message.ReceiverId : message.SenderId;
            if (targetUserId == null || latestMessagesByUser.ContainsKey(targetUserId.Value))
                continue;

            latestMessagesByUser[targetUserId.Value] = message;
        }

        var users = await _unitOfWork.Users.GetBySetIds(latestMessagesByUser.Keys
            .ToList());

        var userById = users.ToDictionary(user => user.Id);

        return latestMessagesByUser
            .Where(pair => userById.ContainsKey(pair.Key))
            .Select(pair => new PrivateConversationDto
            {
                User = _mapper.Map<ShortUserDto>(userById[pair.Key]),
                LastMessageContent = pair.Value.Content,
                LastMessageSentAt = pair.Value.SentAt
            })
            .OrderByDescending(msg => msg.LastMessageSentAt)
            .ToList();
    }

    public async Task<ChatMessageDto> SendGameRoomMessageAsync(GameRoomMessageDto message)
    {
        var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId)
            ?? throw new KeyNotFoundException($"User not found");
        _ = await _unitOfWork.GameRooms.GetByIdAsync(message.GameRoomId)
            ?? throw new KeyNotFoundException($"GameRoom not found");

        var chatMessage = _mapper.Map<ChatMessage>(message);
        chatMessage.Sender = sender;
        await _unitOfWork.Messages.AddAsync(chatMessage);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<ChatMessageDto>(chatMessage);
    }

    public async Task<ChatMessageDto> SendMessageAsync(SendMessageDto message)
    {
        var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId)
            ?? throw new KeyNotFoundException($"User not found");

        var chatMessage = _mapper.Map<ChatMessage>(message);
        chatMessage.Sender = sender;
        await _unitOfWork.Messages.AddAsync(chatMessage);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<ChatMessageDto>(chatMessage);
    }

    public async Task<ChatMessageDto> SendPrivateMessageAsync(PrivateMessageDto message)
    {
        var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId)
            ?? throw new KeyNotFoundException($"Sender User not found");
        _ = await _unitOfWork.Users.GetByIdAsync(message.ReceiverId)
            ?? throw new KeyNotFoundException($"Receiver User not found");

        var chatMessage = _mapper.Map<ChatMessage>(message);
        chatMessage.Sender = sender;
        await _unitOfWork.Messages.AddAsync(chatMessage);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<ChatMessageDto>(chatMessage);
    }
}
