using Backend.Application.DTOs.GameRooms;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Utils.WebApi;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers;

[ApiController]
[Route("admin/game-rooms")]
public class GameRoomController(IGameRoomService gameRoomService) : ControllerBase
{
    private readonly IGameRoomService _gameRoomService = gameRoomService;
    private const int MaxPageSize = 50;

    [HttpGet]
    public async Task<IActionResult> GetGameRooms(int page = 1, int pageSize = 10, RoomStatus? status = null)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var gameRooms = await _gameRoomService.GetGameRoomsAsync(page, pageSize, status);
        return Ok(gameRooms);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGameRoom([FromBody] CreateGameRoomDto room)
    {
        var createdRoom = await _gameRoomService.CreateGameRoomAsync(room.UserId);
        return CreatedAtAction(nameof(GetGameRoomById), new { gameRoomId = createdRoom.Id }, createdRoom);
    }

    [HttpGet("{gameRoomId}")]
    public async Task<IActionResult> GetGameRoomById(Guid gameRoomId)
    {
        var gameRoom = await _gameRoomService.GetGameRoomByIdAsync(gameRoomId);
        return Ok(gameRoom);
    }

    [HttpPost("{gameRoomId}/add-player")]
    public async Task<IActionResult> AddPlayerToGameRoom(Guid gameRoomId, [FromBody] CreateGameRoomDto playerDto)
    {
        await _gameRoomService.AddPlayerToGameRoomAsync(gameRoomId, playerDto.UserId);
        return Ok();
    }

    [HttpPost("{gameRoomId}/remove-player")]
    public async Task<IActionResult> RemovePlayerFromGameRoom(Guid gameRoomId, [FromBody] CreateGameRoomDto playerDto)
    {
        await _gameRoomService.RemovePlayerFromGameRoomAsync(gameRoomId, playerDto.UserId);
        return Ok();
    }

    [HttpDelete("{gameRoomId}/cancel")]
    public async Task<IActionResult> CloseGameRoom(Guid gameRoomId)
    {
        await _gameRoomService.CancelGameRoomAsync(gameRoomId);
        return Ok();
    }

    [HttpPut("{gameRoomId}/player-deck/{playerId}")]
    public async Task<IActionResult> UpdatePlayerDeck(Guid gameRoomId, Guid playerId, [FromBody] ChangeRoomDeckDto dto)
    {
        await _gameRoomService.UpdatePlayerDeckAsync(gameRoomId, playerId, dto.DeckId);
        return Ok();
    }

    [HttpPut("{gameRoomId}/start")]
    public async Task<IActionResult> StartGameInRoom(Guid gameRoomId)
    {
        await _gameRoomService.StartGameFromRoomAsync(gameRoomId);
        return Ok();
    }
}
