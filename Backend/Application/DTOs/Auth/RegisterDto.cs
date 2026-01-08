namespace Backend.Application.DTOs.Auth;

public class RegisterDto
{
    public string Username { get; init ;} = null!;
    public string Email { get; init; } = null!;
    public string PlainPassword { get; init; } = null!;
}