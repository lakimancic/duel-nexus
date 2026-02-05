using Backend.Data.Enums;

namespace Backend.Application.DTOs.Users;

public class EditUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
    public double Elo { get; set; }
}
