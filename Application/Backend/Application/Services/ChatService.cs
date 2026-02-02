using AutoMapper;
using Backend.Application.DTOs.Chat;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;
using Microsoft.VisualBasic;

namespace Backend.Application.Services;

public class ChatService(IUnitOfWork unitOfWork, IMapper mapper) : IChatService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId)
            ?? throw new KeyNotFoundException($"MEssage with ID '{messageId}' not found");
        _unitOfWork.Messages.Delete(message);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PagedResult<ChatMessageDto>> GetGameRoomMessagesAsync(Guid gameRoomId, int page, int pageSize)
    {
        var messages = await _unitOfWork.Messages.GetPagedAsync(
            page, pageSize, msg => msg.GameRoomId == gameRoomId, q => q.OrderByDescending(m => m.SentAt), "Sender");
        return _mapper.Map<PagedResult<ChatMessageDto>>(messages);
    }

    public async Task<PagedResult<ChatMessageDto>> GetGlobalMessagesAsync(int page, int pageSize)
    {
        var messages = await _unitOfWork.Messages.GetPagedAsync(
            page, pageSize, orderBy: q => q.OrderByDescending(m => m.SentAt), includeProperties: "Sender");
        return _mapper.Map<PagedResult<ChatMessageDto>>(messages);
    }

    public async Task<PagedResult<ChatMessageDto>> GetPrivateMessagesAsync(Guid userId1, Guid userId2, int page, int pageSize)
    {
        var messages = await _unitOfWork.Messages.GetPagedAsync(
            page, pageSize, msg => msg.ReceiverId == userId1 || msg.ReceiverId == userId2,
            q => q.OrderByDescending(m => m.SentAt), "Sender");
        return _mapper.Map<PagedResult<ChatMessageDto>>(messages);
    }

    public async Task SendGameRoomMessageAsync(GameRoomMessageDto message)
    {
        _ = await _unitOfWork.Users.GetByIdAsync(message.SenderId)
            ?? throw new KeyNotFoundException($"User with ID '{message.SenderId}' not found");
        _ = await _unitOfWork.GameRooms.GetByIdAsync(message.GameRoomId)
            ?? throw new KeyNotFoundException($"GameRoom with ID '{message.GameRoomId}' not found");

        var chatMessage = _mapper.Map<ChatMessage>(message);
        await _unitOfWork.Messages.AddAsync(chatMessage);
        await _unitOfWork.CompleteAsync();
    }

    public async Task SendMessageAsync(SendMessageDto message)
    {
        _ = await _unitOfWork.Users.GetByIdAsync(message.SenderId)
            ?? throw new KeyNotFoundException($"User with ID '{message.SenderId}' not found");

        var chatMessage = _mapper.Map<ChatMessage>(message);
        await _unitOfWork.Messages.AddAsync(chatMessage);
        await _unitOfWork.CompleteAsync();
    }

    public async Task SendPrivateMessageAsync(PrivateMessageDto message)
    {
        _ = await _unitOfWork.Users.GetByIdAsync(message.SenderId)
            ?? throw new KeyNotFoundException($"User with ID '{message.SenderId}' not found");
        _ = await _unitOfWork.Users.GetByIdAsync(message.ReceiverId)
            ?? throw new KeyNotFoundException($"User with ID '{message.ReceiverId}' not found");

        var chatMessage = _mapper.Map<ChatMessage>(message);
        await _unitOfWork.Messages.AddAsync(chatMessage);
        await _unitOfWork.CompleteAsync();
    }
}
