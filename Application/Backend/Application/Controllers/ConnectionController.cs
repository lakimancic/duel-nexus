using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;

namespace Backend.Application.Controllers;

[ApiController]
[Route("connections")]
public class ConnectionController(IConnectionService connectionService) : ControllerBase
{
    private readonly IConnectionService _connectionService = connectionService;

    [HttpGet]
    public async Task<IActionResult> GetActiveUsers()
    {
        return Ok(await _connectionService.GetOnlineUsers());
    }
}
