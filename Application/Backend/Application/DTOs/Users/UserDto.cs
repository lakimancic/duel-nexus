using Backend.Data.Enums;

namespace Backend.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }
    public double Elo { get; set; }
}
