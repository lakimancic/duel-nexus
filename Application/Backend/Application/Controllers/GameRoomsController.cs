using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Application.DTOs.GameRooms;
using Backend.Application.Services.Interfaces;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Application.Controllers;

[Authorize]
[ApiController]
[Route("game-rooms")]
public class GameRoomsController(
    IGameRoomService gameRoomService,
    IHubContext<GameHub> hubContext
) : ControllerBase
{
    private readonly IGameRoomService _gameRoomService = gameRoomService;
    private readonly IHubContext<GameHub> _hubContext = hubContext;

    [HttpPost("friendly")]
    public async Task<IActionResult> CreateFriendlyRoom()
    {
        var userId = GetUserId();
        var room = await _gameRoomService.CreateFriendlyRoom(userId);
        return Ok(room);
    }

    [HttpPost("friendly/join")]
    public async Task<IActionResult> JoinFriendlyRoom([FromBody] JoinGameRoomDto joinDto)
    {
        var userId = GetUserId();
        var room = await _gameRoomService.JoinFriendlyRoom(userId, joinDto.JoinCode);

        await _hubContext.Clients.Group(GameHub.GetGameRoomGroupName(room.Id))
            .SendAsync("game-room:players:updated", room.Id);

        return Ok(room);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoomById(Guid id)
    {
        var room = await _gameRoomService.GetGameRoomById(id);
        if (room == null)
            return NotFound(new { error = "Game Room not found" });
        return Ok(room);
    }

    [HttpGet("{id}/players")]
    public async Task<IActionResult> GetRoomPlayers(Guid id)
    {
        var players = await _gameRoomService.GetGameRoomPlayers(id);
        return Ok(players);
    }

    [HttpPut("{id}/deck")]
    public async Task<IActionResult> SetMyDeck(Guid id, [FromBody] EditGameRoomPlayerDto dto)
    {
        var userId = GetUserId();
        var player = await _gameRoomService.SetPlayerGameDeck(id, userId, dto.DeckId);

        await _hubContext.Clients.Group(GameHub.GetGameRoomGroupName(id))
            .SendAsync("game-room:players:updated", id);

        return Ok(player);
    }

    [HttpDelete("{id}/leave")]
    public async Task<IActionResult> LeaveRoom(Guid id)
    {
        var userId = GetUserId();
        var isCancelled = await _gameRoomService.LeaveRoom(id, userId);

        var roomGroup = GameHub.GetGameRoomGroupName(id);
        if (isCancelled)
            await _hubContext.Clients.Group(roomGroup).SendAsync("game-room:cancelled", id);
        else
            await _hubContext.Clients.Group(roomGroup).SendAsync("game-room:players:updated", id);

        return Ok(new { cancelled = isCancelled });
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelRoom(Guid id)
    {
        var userId = GetUserId();
        await _gameRoomService.CancelRoom(id, userId);

        await _hubContext.Clients.Group(GameHub.GetGameRoomGroupName(id))
            .SendAsync("game-room:cancelled", id);

        return Ok(new { message = "Room canceled successfully." });
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartRoomGame(Guid id)
    {
        var userId = GetUserId();
        var gameId = await _gameRoomService.StartRoomGame(id, userId);
        return Ok(new { gameId });
    }

    private Guid GetUserId()
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var nameIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userIdString = subClaim ?? nameIdClaim;
        if (string.IsNullOrWhiteSpace(userIdString))
            throw new BadRequestException("User ID not found in token.");

        return Guid.Parse(userIdString);
    }
}
