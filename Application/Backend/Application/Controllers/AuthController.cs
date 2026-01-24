using Microsoft.AspNetCore.Mvc;
using Backend.Application.DTOs.Auth;
using Backend.Application.Services.Interfaces;
using Backend.Utils.Security;

namespace Backend.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IUserService userService, JwtTokenGenerator jwtGenerator) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly JwtTokenGenerator _jwtGenerator = jwtGenerator;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        if (await _userService.ExistsAsync(request.Email))
            return BadRequest(new { error = "User already exists" });

        var user = await _userService.RegisterAsync(request);
        var token = _jwtGenerator.Generate(user);

        return Ok(new { token, userId = user.Id, role = user.Role.ToString() });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var user = await _userService.LoginAsync(request);
        if (user == null)
            return BadRequest(new { error = "Invalid credentials" });

        var token = _jwtGenerator.Generate(user);

        return Ok(new { token, userId = user.Id, role = user.Role.ToString() });
    }
}
