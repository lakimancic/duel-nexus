namespace Backend.Api.Models.Requests;

public class RegisterRequest
{
    public string Username { get; init ;} = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}