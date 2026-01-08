namespace Backend.Api.Models.Requests;

public class LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}