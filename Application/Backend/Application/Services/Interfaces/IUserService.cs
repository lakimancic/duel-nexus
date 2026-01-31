using Backend.Application.DTOs.Auth;
using Backend.Application.DTOs.Users;

namespace Backend.Application.Services.Interfaces;

public interface IUserService
{
    public Task<bool> ExistsAsync(string email);
    public Task<UserDto> RegisterAsync(RegisterDto registerDto);
    public Task<UserDto?> LoginAsync(LoginDto loginDto);
}
