using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.GameRooms;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers;

[ApiController]
[Route("game-rooms")]
public class GameRoomController(IGameRoomService gameRoomService) : ControllerBase
{
    private readonly IGameRoomService _gameRoomService = gameRoomService;

    [HttpPost("")]
    public async Task<IActionResult> CreateGameRoom([FromBody] CreateGameRoomRequest request)
    {
        try {
            var gameRoom = await _gameRoomService.CreateGameRoomAsync(request.UserId, request.DeckId);
            return Ok(gameRoom);
        }
        catch (NotFoundException ex) {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllGameRooms()
    {
        var gameRooms = await _gameRoomService.GetAllGameRoomsAsync();
        return Ok(gameRooms);
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinGameRoom([FromBody] JoinGameRoomRequest request)
    {
        try {
            var gameRoomId = await _gameRoomService.JoinGameRoomAsync(request.Code, request.UserId, request.DeckId);
            return Ok(gameRoomId);
        }
        catch (NotFoundException ex) {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("leave")]
    public async Task<IActionResult> LeaveGameRoom([FromBody] LeaveGameRoomRequest request)
    {
        try {
            await _gameRoomService.LeaveGameRoomAsync(request.GameRoomId, request.UserId);
            return Ok();
        }
        catch (NotFoundException ex) {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex) {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
