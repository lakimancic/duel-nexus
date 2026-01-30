using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.GameRooms;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Application.Controllers;

[ApiController]
[Route("game-rooms")]
public class GameRoomController(IGameRoomService gameRoomService) : ControllerBase
{
    private readonly IGameRoomService _gameRoomService = gameRoomService;

    private IActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => NotFound(new { error = ex.Message }),
            ArgumentException => BadRequest(new { error = ex.Message }),
            InvalidOperationException => Conflict(new { error = ex.Message }),
            _ => StatusCode(500, new { error = ex.Message })
        };
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateGameRoom([FromBody] CreateGameRoomRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var gameRoom = await _gameRoomService.CreateGameRoomAsync(request.UserId, request.DeckId);
            return CreatedAtAction(nameof(GetGameRoom), new { id = gameRoom.Id }, gameRoom);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGameRoom(Guid id)
    {
        try
        {
            var gameRoom = await _gameRoomService.GetGameRoomByIdAsync(id);
            if (gameRoom == null)
                return NotFound(new { error = "Game room not found" });
            return Ok(gameRoom);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllGameRooms()
    {
        try
        {
            var gameRooms = await _gameRoomService.GetAllGameRoomsAsync();
            return Ok(gameRooms);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost("add-player")]
    public async Task<IActionResult> AddPlayerGameRoom([FromBody] JoinGameRoomRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var gameRoomId = await _gameRoomService.JoinGameRoomAsync(request.Code, request.UserId, request.DeckId);
            return Ok(new { gameRoomId });
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost("remove-player")]
    public async Task<IActionResult> RemovePlayerGameRoom([FromBody] LeaveGameRoomRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _gameRoomService.LeaveGameRoomAsync(request.GameRoomId, request.UserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}
