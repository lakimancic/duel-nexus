using Backend.Application.DTOs.Users;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Utils.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers.Admin;

[Route("admin/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private const int MaxPageSize = 50;

    [HttpGet]
    public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 20, string? search = null)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var users = await _userService.GetUsers(page, pageSize, search);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
            return NotFound(new { error = "User not found" });
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] EditUserDto userDto)
    {
        await _userService.EditUser(id, userDto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUser(id);
        return Ok(new { message = "User deleted successfully" });
    }

    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        return Ok(EnumExtensions.GetNameValues<UserRole>());
    }

    [HttpGet("{id}/cards")]
    public async Task<IActionResult> GetPlayerCards(Guid id, int page = 1, int pageSize = 10, string? search = null)
    {
        var playerCards = await _userService.GetPlayerCards(id, page, pageSize, search);
        return Ok(playerCards);
    }

    [HttpPost("{id}/cards")]
    public async Task<IActionResult> AddPlayerCard(Guid id, [FromBody] CreatePlayerCardDto playerCardDto)
    {
        var playerCard = await _userService.CreatePlayerCard(id, playerCardDto);
        return Ok(playerCard);
    }

    [HttpPut("{id}/cards/{cardId}")]
    public async Task<IActionResult> UpdatePlayerCard(Guid id, Guid cardId, [FromBody] EditPlayerCardDto editPlayerCard)
    {
        var playerCard = await _userService.EditPlayerCard(id, cardId, editPlayerCard);
        return Ok(playerCard);
    }

    [HttpDelete("{id}/cards/{cardId}")]
    public async Task<IActionResult> RemovePlayerCard(Guid id, Guid cardId)
    {
        await _userService.DeletePlayerCard(id, cardId);
        return Ok(new { message = "Player Card deleted successfully" });
    }
}
