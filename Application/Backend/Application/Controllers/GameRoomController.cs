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
        try
        {
            var gameRooms = await _gameRoomService.GetGameRoomsAsync(page, pageSize, status);
            return Ok(gameRooms);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.HandleException(ex, this);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGameRoom([FromBody] CreateGameRoomDto room)
    {
        try
        {
            var createdRoom = await _gameRoomService.CreateGameRoomAsync(room.UserId);
            return Ok(createdRoom);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.HandleException(ex, this);
        }
    }

    [HttpGet("{gameRoomId}")]
    public async Task<IActionResult> GetGameRoomById(Guid gameRoomId)
    {
        try
        {
            var gameRoom = await _gameRoomService.GetGameRoomByIdAsync(gameRoomId);
            return Ok(gameRoom);
        }
        catch (Exception ex)
        {
            return ExceptionHandler.HandleException(ex, this);
        }
    }

    [HttpPost("{gameRoomId}/add-player")]
    public async Task<IActionResult> AddPlayerToGameRoom(Guid gameRoomId, [FromBody] CreateGameRoomDto playerDto)
    {
        try
        {
            await _gameRoomService.AddPlayerToGameRoomAsync(gameRoomId, playerDto.UserId);
            return Ok();
        }
        catch (Exception ex)
        {
            return ExceptionHandler.HandleException(ex, this);
        }
    }

    [HttpPost("{gameRoomId}/remove-player")]
    public async Task<IActionResult> RemovePlayerFromGameRoom(Guid gameRoomId, [FromBody] CreateGameRoomDto playerDto)
    {
        try
        {
            await _gameRoomService.RemovePlayerFromGameRoomAsync(gameRoomId, playerDto.UserId);
            return Ok();
        }
        catch (Exception ex)
        {
            return ExceptionHandler.HandleException(ex, this);
        }
    }

    [HttpDelete("{gameRoomId}/cancel")]
    public async Task<IActionResult> CloseGameRoom(Guid gameRoomId)
    {
        try
        {
            await _gameRoomService.CancelGameRoomAsync(gameRoomId);
            return Ok();
        }
        catch (Exception ex)
        {
            return ExceptionHandler.HandleException(ex, this);
        }
    }

    [HttpPut("{gameRoomId}/player-deck/{playerId}")]
    public async Task<IActionResult> UpdatePlayerDeck(Guid gameRoomId, Guid playerId, [FromBody] ChangeRoomDeckDto dto)
    {
        try
        {
            await _gameRoomService.UpdatePlayerDeckAsync(gameRoomId, playerId, dto.DeckId);
            return Ok();
        }
        catch (Exception ex)
        {
            return ExceptionHandler.HandleException(ex, this);
        }
    }
}
