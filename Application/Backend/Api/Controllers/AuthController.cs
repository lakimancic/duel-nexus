using Backend.Application.Services;
using Backend.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Application.DTOs.Auth;

namespace Backend.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IUserService userService, PasswordHasher passwordHasher, JwtTokenGenerator jwtGenerator) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly PasswordHasher _passwordHasher = passwordHasher;
    private readonly JwtTokenGenerator _jwtGenerator = jwtGenerator;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        if (await _userService.ExistsAsync(request.Email))
            return BadRequest(new { error = "User already exists" });

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(Guid.NewGuid(), request.Email, request.Username, passwordHash, UserRole.Player);
        await _userService.AddAsync(user);

        var token = _jwtGenerator.Generate(user);

        return Ok(new { token, userId = user.Id, role = user.Role.ToString() });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var user = await _userService.GetByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return BadRequest(new { error = "Invalid credentials" });

        var token = _jwtGenerator.Generate(user);

        return Ok(new { token, userId = user.Id, role = user.Role.ToString() });
    }
}
