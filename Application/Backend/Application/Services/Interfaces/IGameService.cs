using Backend.Application.DTOs.GameRooms;
using Backend.Application.DTOs.Games;
using Backend.Data.Models;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IGameService
{
    Task<TurnDto> AddNewGameTurn(Guid id);
    Task<AttackActionDto> CreateAttackAction(CreateAttackActionDto actionDto);
    Task<CardMovementDto> CreateCardMovement(CreateCardMovementDto actionDto);
    Task<EffectActivationDto> CreateEffectActivation(CreateEffectActivationDto actionDto);
    Task<GameDto> CreateGame(CreateGameDto gameDto);
    Task<PlaceCardDto> CreatePlaceAction(CreatePlaceActionDto actionDto);
    Task DeleteAttackAction(Guid actionId);
    Task DeleteCardMovement(Guid actionId);
    Task DeleteEffectActivation(Guid actionId);
    Task DeletePlaceAction(Guid actionId);
    Task<AttackActionDto> EditAttackAction(Guid actionId, CreateAttackActionDto actionDto);
    Task<CardMovementDto> EditCardMovement(Guid actionId, CreateCardMovementDto actionDto);
    Task<EffectActivationDto> EditEffectActivation(Guid actionId, CreateEffectActivationDto actionDto);
    Task<GameDto> EditGame(Guid id, EditGameDto gameDto);
    Task<GameCardDto> EditGameCard(Guid cardId, EditGameCardDto gameCardDto);
    Task<TurnDto> EditGameTurn(Guid turnId, EditTurnDto turnDto);
    Task<PlaceCardDto> EditPlaceAction(Guid actionId, CreatePlaceActionDto actionDto);
    Task<PlayerGameDto> EditPlayerGame(Guid id, Guid userId, EditPlayerGameDto playerGameDto);
    Task<AttackActionDto?> GetAttackAction(Guid actionId);
    Task<PagedResult<AttackActionDto>> GetAttackActions(Guid id, int page, int pageSize);
    Task<CardMovementDto?> GetCardMovement(Guid actionId);
    Task<PagedResult<CardMovementDto>> GetCardMovements(Guid id, int page, int pageSize);
    Task<EffectActivationDto?> GetEffectActivation(Guid actionId);
    Task<PagedResult<EffectActivationDto>> GetEffectActivations(Guid id, int page, int pageSize);
    Task<GameDto?> GetGameById(Guid id);
    Task<GameCardDto?> GetGameCard(Guid cardId);
    Task<List<GameCardDto>> GetGameCards(Guid id);
    Task<PlayerGameDto> GetGamePlayer(Guid id, Guid userId);
    Task<List<PlayerGameDto>> GetGamePlayers(Guid id);
    Task<PagedResult<GameDto>> GetGames(int page, int pageSize);
    Task<TurnDto?> GetGameTurn(Guid turnId);
    Task<PagedResult<TurnDto>> GetGameTurns(Guid id, int page, int pageSize);
    Task<PlaceCardDto?> GetPlaceAction(Guid actionId);
    Task<PagedResult<PlaceCardDto>> GetPlaceActions(Guid id, int page, int pageSize);
}
