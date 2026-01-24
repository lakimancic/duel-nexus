using System.ComponentModel.DataAnnotations;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.Player;
    public double Elo { get; set; } = 1000.0;
}
