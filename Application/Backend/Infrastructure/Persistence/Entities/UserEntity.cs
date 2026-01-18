using Backend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Persistence.Entities;

public class UserEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
}
