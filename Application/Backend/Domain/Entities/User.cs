using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public class User(Guid id, string email, string username, string passwordHash, UserRole role)
{
    public Guid Id { get; private set; } = id;
    public string Username { get; private set; } = username;
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;
    public UserRole Role { get; private set; } = role;
}
