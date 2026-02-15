using AutoMapper;
using Backend.Application.DTOs.Users;
using Backend.Application.Services.Interfaces;
using Backend.Data.UnitOfWork;

namespace Backend.Application.Services;

public class ConnectionService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ConnectionTracker connectionTracker
    ) : IConnectionService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ConnectionTracker _connectionTracker = connectionTracker;

    public Guid? AddOnlineUser(Guid userId, string connectionId)
    {
        return _connectionTracker.AddOnlineUser(userId, connectionId);
    }

    public List<string> GetConnectionByUser(Guid userId)
    {
        return _connectionTracker.GetConnectionByUser(userId);
    }

    public async Task<List<ShortUserDto>> GetOnlineUsers()
    {
        var onlineUserIds = _connectionTracker.GetOnlineUserIds();
        if (!onlineUserIds.Any())
            return [];

        var users = await _unitOfWork.Users.GetBySetIds(onlineUserIds);

        return _mapper.Map<List<ShortUserDto>>(users);
    }

    public Guid? RemoveOnlineUser(Guid userId, string connectionId)
    {
        return _connectionTracker.RemoveOnlineUser(userId, connectionId);
    }
}
