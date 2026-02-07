using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.GameRooms;
using Backend.Utils.Data;
using Backend.Data.Enums;

namespace Backend.Application.Controllers.Admin;

[ApiController]
[Route("admin/game-rooms")]
public class GameRoomsController(IGameRoomService gameRoomService) : ControllerBase
{
    private readonly IGameRoomService _gameRoomService = gameRoomService;
    private const int MaxPageSize = 50;

    [HttpGet]
    public async Task<IActionResult> GetGameRooms(int page = 1, int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var rooms = await _gameRoomService.GetRooms(page, pageSize);
        return Ok(rooms);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGameRoom([FromBody] CreateGameRoomDto gameRoomDto)
    {
        var createdGameRoom = await _gameRoomService.CreateRoom(gameRoomDto);
        return Ok(createdGameRoom);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameRoomById(Guid id)
    {
        var gameRoom = await _gameRoomService.GetGameRoomById(id);
        if (gameRoom == null)
            return NotFound(new { error = "Game Room not found" });
        return Ok(gameRoom);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGameRoom(Guid id, [FromBody] EditGameRoomDto gameRoomDto)
    {
        var updatedGameRoom = await _gameRoomService.EditGameRoom(id, gameRoomDto);
        return Ok(updatedGameRoom);
    }

    [HttpGet("{id}/players")]
    public async Task<IActionResult> GetGameRoomPlayers(Guid id)
    {
        var players = await _gameRoomService.GetGameRoomPlayers(id);
        return Ok(players);
    }

    [HttpPost("{id}/players")]
    public async Task<IActionResult> AddPlayerToGameRoom(Guid id, [FromBody] InsertGameRoomPlayerDto playerDto)
    {
        var player = await _gameRoomService.AddPlayerToGameRoom(id, playerDto);
        return Ok(player);
    }

    [HttpPut("{id}/players/{userId}")]
    public async Task<IActionResult> SetPlayerDeck(Guid id, Guid userId, [FromBody] EditGameRoomPlayerDto playerDto)
    {
        var updatedPlayer = await _gameRoomService.SetPlayerGameDeck(id, userId, playerDto.DeckId);
        return Ok(updatedPlayer);
    }

    [HttpDelete("{id}/players/{userId}")]
    public async Task<IActionResult> RemovePlayerFromGameRoom(Guid id, Guid userId)
    {
        await _gameRoomService.DeleteGameRoomPlayer(id, userId);
        return Ok(new { message = "Game Room Player deleted successfully" });
    }

    [HttpGet("statuses")]
    public IActionResult GetTypes()
    {
        return Ok(EnumExtensions.GetNameValues<RoomStatus>());
    }
}
