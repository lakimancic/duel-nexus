using Backend.Application.DTOs.Users;

namespace Backend.Application.Services.Interfaces;

public interface IConnectionService
{
    List<string> GetConnectionByUser(Guid userId);
    Guid? AddOnlineUser(Guid userId, string connectionId);
    Guid? RemoveOnlineUser(Guid userId, string connectionId);
    Task<List<ShortUserDto>> GetOnlineUsers();
}
