using Microsoft.AspNetCore.Mvc;
using Backend.Application.Services.Interfaces;
using Backend.Application.DTOs.Games;
using Backend.Utils.Data;
using Backend.Data.Enums;

namespace Backend.Application.Controllers.Admin;

[ApiController]
[Route("admin/games")]
public class GamesController(IGameService gameService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;
    private const int MaxPageSize = 50;

    [HttpGet]
    public async Task<IActionResult> GetGame(int page = 1, int pageSize = 20)
    {
        pageSize = Math.Min(pageSize, MaxPageSize);
        var rooms = await _gameService.GetGames(page, pageSize);
        return Ok(rooms);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameDto gameDto)
    {
        var createdGame = await _gameService.CreateGame(gameDto);
        return Ok(createdGame);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await _gameService.GetGameById(id);
        if (game == null)
            return NotFound(new { error = "Game not found" });
        return Ok(game);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(Guid id, [FromBody] EditGameDto gameDto)
    {
        var updatedGame = await _gameService.EditGame(id, gameDto);
        return Ok(updatedGame);
    }

    [HttpGet("{id}/players")]
    public async Task<IActionResult> GetGamePlayers(Guid id)
    {
        var players = await _gameService.GetGamePlayers(id);
        return Ok(players);
    }

    [HttpGet("{id}/players/{userId}")]
    public async Task<IActionResult> GetGamePlayer(Guid id, Guid userId)
    {
        var gamePlayer = await _gameService.GetGamePlayer(id, userId);
        if (gamePlayer == null)
            return NotFound(new { error = "Game Player not found" });
        return Ok(gamePlayer);
    }

    [HttpPut("{id}/players/{userId}")]
    public async Task<IActionResult> UpdateGamePlayer(Guid id, Guid userId, [FromBody] EditPlayerGameDto playerGameDto)
    {
        var updatedPlayer = await _gameService.EditPlayerGame(id, userId, playerGameDto);
        return Ok(updatedPlayer);
    }

    [HttpGet("{id}/turns")]
    public async Task<IActionResult> GetGameTurns(Guid id, int page = 1, int pageSize = 20)
    {
        var turns = await _gameService.GetGameTurns(id, page, pageSize);
        return Ok(turns);
    }

    [HttpPost("{id}/turns")]
    public async Task<IActionResult> AddNewTurn(Guid id)
    {
        var newTurn = await _gameService.AddNewGameTurn(id);
        return Ok(newTurn);
    }

    [HttpGet("turns/{turnId}")]
    public async Task<IActionResult> GetGameTurn(Guid turnId)
    {
        var turn = await _gameService.GetGameTurn(turnId);
        if (turn == null)
            return NotFound(new { error = "Game Turn not found" });
        return Ok(turn);
    }

    [HttpPut("turns/{turnId}")]
    public async Task<IActionResult> UpdateTurn(Guid turnId, [FromBody] EditTurnDto turnDto)
    {
        var updatedTurn = await _gameService.EditGameTurn(turnId, turnDto);
        return Ok(updatedTurn);
    }

    [HttpGet("{id}/cards")]
    public async Task<IActionResult> GetGameCards(Guid id)
    {
        var cards = await _gameService.GetGameCards(id);
        return Ok(cards);
    }

    [HttpGet("cards/{cardId}")]
    public async Task<IActionResult> GetGameCard(Guid cardId)
    {
        var gameCard = await _gameService.GetGameCard(cardId);
        if (gameCard == null)
            return NotFound(new { error = "Game Card not found" });
        return Ok(gameCard);
    }

    [HttpPut("cards/{cardId}")]
    public async Task<IActionResult> UpdateGameCard(Guid cardId, [FromBody] EditGameCardDto gameCardDto)
    {
        var updatedCard = await _gameService.EditGameCard(cardId, gameCardDto);
        return Ok(updatedCard);
    }

    [HttpGet("{id}/attack-actions")]
    public async Task<IActionResult> GetAttackActions(Guid id, int page = 1, int pageSize = 20)
    {
        var actions = await _gameService.GetAttackActions(id, page, pageSize);
        return Ok(actions);
    }

    [HttpPost("attack-actions")]
    public async Task<IActionResult> CreateAttackAction([FromBody] CreateAttackActionDto actionDto)
    {
        var newAction = await _gameService.CreateAttackAction(actionDto);
        return Ok(newAction);
    }

    [HttpGet("attack-actions/{actionId}")]
    public async Task<IActionResult> GetAttackAction(Guid actionId)
    {
        var action = await _gameService.GetAttackAction(actionId);
        if (action == null)
            return NotFound(new { error = "Attack Action not found" });
        return Ok(action);
    }

    [HttpPut("attack-actions/{actionId}")]
    public async Task<IActionResult> UpdateAttackAction(Guid actionId, [FromBody] CreateAttackActionDto actionDto)
    {
        var updatedAction = await _gameService.EditAttackAction(actionId, actionDto);
        return Ok(updatedAction);
    }

    [HttpDelete("attack-actions/{actionId}")]
    public async Task<IActionResult> DeleteAttackAction(Guid actionId)
    {
        await _gameService.DeleteAttackAction(actionId);
        return Ok(new { message = "Attack Action deleted successfully" });
    }

    [HttpGet("{id}/card-movements")]
    public async Task<IActionResult> GetCardMovements(Guid id, int page = 1, int pageSize = 20)
    {
        var movements = await _gameService.GetCardMovements(id, page, pageSize);
        return Ok(movements);
    }

    [HttpPost("card-movements")]
    public async Task<IActionResult> CreateCardMovement(Guid id, [FromBody] CreateCardMovementDto actionDto)
    {
        var newAction = await _gameService.CreateCardMovement(id, actionDto);
        return Ok(newAction);
    }

    [HttpGet("card-movements/{actionId}")]
    public async Task<IActionResult> GetCardMovement(Guid actionId)
    {
        var action = await _gameService.GetCardMovement(actionId);
        if (action == null)
            return NotFound(new { error = "Card Movement not found" });
        return Ok(action);
    }

    [HttpPut("card-movements/{actionId}")]
    public async Task<IActionResult> UpdateCardMovement(Guid actionId, [FromBody] CreateCardMovementDto actionDto)
    {
        var updatedAction = await _gameService.EditCardMovement(actionId, actionDto);
        return Ok(updatedAction);
    }

    [HttpDelete("card-movements/{actionId}")]
    public async Task<IActionResult> DeleteCardMovement(Guid actionId)
    {
        await _gameService.DeleteCardMovement(actionId);
        return Ok(new { message = "Card Movement deleted successfully" });
    }

    [HttpGet("{id}/effect-activations")]
    public async Task<IActionResult> GetEffectActivations(Guid id, int page = 1, int pageSize = 20)
    {
        var activations = await _gameService.GetEffectActivations(id, page, pageSize);
        return Ok(activations);
    }

    [HttpPost("effect-activations")]
    public async Task<IActionResult> CreateEffectActivation(Guid id, [FromBody] CreateEffectActivationDto actionDto)
    {
        var newAction = await _gameService.CreateEffectActivation(id, actionDto);
        return Ok(newAction);
    }

    [HttpGet("effect-activations/{actionId}")]
    public async Task<IActionResult> GetEffectActivation(Guid actionId)
    {
        var action = await _gameService.GetEffectActivation(actionId);
        if (action == null)
            return NotFound(new { error = "Effect Activation not found" });
        return Ok(action);
    }

    [HttpPut("effect-activations/{actionId}")]
    public async Task<IActionResult> UpdateEffectActivation(Guid actionId, [FromBody] CreateEffectActivationDto actionDto)
    {
        var updatedAction = await _gameService.EditEffectActivation(actionId, actionDto);
        return Ok(updatedAction);
    }

    [HttpDelete("effect-activations/{actionId}")]
    public async Task<IActionResult> DeleteEffectActivation(Guid actionId)
    {
        await _gameService.DeleteEffectActivation(actionId);
        return Ok(new { message = "Effect Activation deleted successfully" });
    }

    [HttpGet("{id}/place-actions")]
    public async Task<IActionResult> GetPlaceActions(Guid id, int page = 1, int pageSize = 20)
    {
        var actions = await _gameService.GetPlaceActions(id, page, pageSize);
        return Ok(actions);
    }

    [HttpPost("place-actions")]
    public async Task<IActionResult> CreatePlaceAction(Guid id, [FromBody] CreatePlaceActionDto actionDto)
    {
        var newAction = await _gameService.CreatePlaceAction(id, actionDto);
        return Ok(newAction);
    }

    [HttpGet("place-actions/{actionId}")]
    public async Task<IActionResult> GetPlaceAction(Guid actionId)
    {
        var action = await _gameService.GetPlaceAction(actionId);
        if (action == null)
            return NotFound(new { error = "Place Card Action not found" });
        return Ok(action);
    }

    [HttpPut("place-actions/{actionId}")]
    public async Task<IActionResult> UpdatePlaceAction(Guid actionId, [FromBody] CreatePlaceActionDto actionDto)
    {
        var updatedAction = await _gameService.EditPlaceAction(actionId, actionDto);
        return Ok(updatedAction);
    }

    [HttpDelete("place-actions/{actionId}")]
    public async Task<IActionResult> DeletePlaceAction(Guid actionId)
    {
        await _gameService.DeletePlaceAction(actionId);
        return Ok(new { message = "Place Action deleted successfully" });
    }

    [HttpGet("movement-types")]
    public IActionResult GetMovementTypes()
    {
        return Ok(EnumExtensions.GetNameValues<CardMovementType>());
    }

    [HttpGet("card-zones")]
    public IActionResult GetCardZones()
    {
        return Ok(EnumExtensions.GetNameValues<CardZone>());
    }

    [HttpGet("place-types")]
    public IActionResult GetPlaceTypes()
    {
        return Ok(EnumExtensions.GetNameValues<PlaceType>());
    }

    [HttpGet("turn-phases")]
    public IActionResult GetTurnPhases()
    {
        return Ok(EnumExtensions.GetNameValues<TurnPhase>());
    }
}
