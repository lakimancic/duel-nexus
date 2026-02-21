using AutoMapper;
using Backend.Application.DTOs.Cards;
using Backend.Application.DTOs.Games;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Domain.Commands;
using Backend.Domain.Engine;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;
using Backend.Utils.WebApi;

namespace Backend.Application.Services;

public class GameService(IUnitOfWork unitOfWork, IMapper mapper, IGameEngine gameEngine) : IGameService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IGameEngine _gameEngine = gameEngine;

    public async Task<TurnDto> AddNewGameTurn(Guid id)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Game not found");

        var current = await _unitOfWork.Turns.GetCurrentTurnAsync(id);
        Turn turn;
        if (current == null)
        {
            turn = await _unitOfWork.Turns.InitializeTurnsForGameAsync(game);
            await _unitOfWork.CompleteAsync();
        }
        else
        {
            turn = await _unitOfWork.Turns.NextTurnAsync(current);
            await _unitOfWork.CompleteAsync();
        }

        return _mapper.Map<TurnDto>(turn);
    }

    public async Task<AttackActionDto> CreateAttackAction(CreateAttackActionDto actionDto)
    {
        var turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");

        var attacker = await _unitOfWork.GameCards.GetByWithCardById(actionDto.AttackerCardId)
            ?? throw new ObjectNotFoundException("Attacker Card not found");

        GameCard? defender = null;
        if (actionDto.DefenderCardId.HasValue)
            defender = await _unitOfWork.GameCards.GetByWithCardById(actionDto.DefenderCardId.Value)
                ?? throw new ObjectNotFoundException("Defender Card not found");

        PlayerGame? defenderPlayer = null;
        if (actionDto.DefenderPlayerGameId.HasValue)
            defenderPlayer = await _unitOfWork.PlayerGames.GetByWithUserById(actionDto.DefenderPlayerGameId.Value)
                ?? throw new ObjectNotFoundException("Defender Player not found");

        var action = _mapper.Map<AttackAction>(actionDto);
        action.Turn = turn;
        action.Attacker = attacker;
        action.Defender = defender;
        action.DefenderPlayer = defenderPlayer;

        await _unitOfWork.Attacks.AddAsync(action);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<AttackActionDto>(action);
    }

    public async Task<CardMovementDto> CreateCardMovement(CreateCardMovementDto actionDto)
    {
        var turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");
        var card = await _unitOfWork.GameCards.GetByWithCardById(actionDto.GameCardId)
            ?? throw new ObjectNotFoundException("Game Card not found");

        var action = _mapper.Map<CardMovementAction>(actionDto);
        action.Turn = turn;
        action.Card = card;

        await _unitOfWork.CardMovements.AddAsync(action);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CardMovementDto>(action);
    }

    public async Task<EffectActivationDto> CreateEffectActivation(CreateEffectActivationDto actionDto)
    {
        var turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");
        var effect = await _unitOfWork.Effects.GetByIdAsync(actionDto.EffectId)
            ?? throw new ObjectNotFoundException("Effect not found");
        var card = await _unitOfWork.GameCards.GetByWithCardById(actionDto.SourceCardId)
            ?? throw new ObjectNotFoundException("Source Card not found");

        var activation = _mapper.Map<EffectActivation>(actionDto);
        activation.Turn = turn;
        activation.Effect = effect;
        activation.SourceCard = card;

        await _unitOfWork.EffectActivations.AddAsync(activation);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<EffectActivationDto>(activation);
    }

    public async Task<GameDto> CreateGame(CreateGameDto gameDto)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetWithHostById(gameDto.RoomId)
            ?? throw new ObjectNotFoundException("Game Room not found");

        var roomPlayers = await _unitOfWork.GameRoomPlayers.GetByGameRoomId(gameDto.RoomId);
        if (roomPlayers == null || roomPlayers.Count == 0)
            throw new BadRequestException("Game room has no players");

        var game = await _unitOfWork.Games.CreateGameFromRoomAsync(gameRoom);

        for (int i = 0; i < roomPlayers.Count; i++)
        {
            var grp = roomPlayers[i];
            if (!grp.DeckId.HasValue)
                throw new BadRequestException($"Player {grp.UserId} has no deck assigned");

            var pg = await _unitOfWork.PlayerGames.CreatePlayerAsync(grp, game, i);
            var deckCards = await _unitOfWork.DeckCards.GetByDeckId(grp.DeckId!.Value);
            await _unitOfWork.GameCards.CreateGameCardsAsync(pg, deckCards);
        }

        await _unitOfWork.Turns.InitializeTurnsForGameAsync(game);
        await _gameEngine.InitializeGameAsync(game);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<GameDto>(game);
    }

    public async Task<DrawActionResultDto> DrawCard(Guid gameId, Guid userId)
    {
        var drawResult = await _gameEngine.ExecuteCommandAsync(gameId, userId, new DrawActionCommand());
        var viewerState = await _gameEngine.GetGameStateAsync(gameId, userId);
        var drawnCard = viewerState.Cards.FirstOrDefault(card => card.Id == drawResult.DrawnCard.Id)
            ?? throw new ObjectNotFoundException("Drawn card not found in game state");

        return new DrawActionResultDto
        {
            GameId = drawResult.Game.Id,
            RoomId = drawResult.Game.RoomId,
            PlayerGameId = drawResult.Player.Id,
            TurnId = drawResult.Turn.Id,
            DrawsInTurn = drawResult.DrawsInTurn,
            TurnEnded = drawResult.TurnEnded,
            NextActivePlayerId = drawResult.NextActivePlayerId,
            DrawnCard = MapCardForViewer(drawnCard, viewerState.Viewer.Id),
        };
    }

    public async Task<PlaceCardDto> CreatePlaceAction(CreatePlaceActionDto actionDto)
    {
        var turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");
        var card = await _unitOfWork.GameCards.GetByWithCardById(actionDto.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found");

        var action = _mapper.Map<PlaceCardAction>(actionDto);
        action.Turn = turn;
        action.Card = card;

        await _unitOfWork.PlaceCards.AddAsync(action);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<PlaceCardDto>(action);
    }

    public async Task DeleteAttackAction(Guid actionId)
    {
        var action = await _unitOfWork.Attacks.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Attack Action not found");
        _unitOfWork.Attacks.Delete(action);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteCardMovement(Guid actionId)
    {
        var action = await _unitOfWork.CardMovements.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Card Movement not found");
        _unitOfWork.CardMovements.Delete(action);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteEffectActivation(Guid actionId)
    {
        var action = await _unitOfWork.EffectActivations.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Effect Activation not found");
        _unitOfWork.EffectActivations.Delete(action);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeletePlaceAction(Guid actionId)
    {
        var action = await _unitOfWork.PlaceCards.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Place Action not found");
        _unitOfWork.PlaceCards.Delete(action);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<AttackActionDto> EditAttackAction(Guid actionId, CreateAttackActionDto actionDto)
    {
        var existing = await _unitOfWork.Attacks.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Attack Action not found");
        var updated = _mapper.Map(actionDto, existing);
        updated.Turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");
        updated.Attacker = await _unitOfWork.GameCards.GetByWithCardById(actionDto.AttackerCardId)
            ?? throw new ObjectNotFoundException("Attacker card not found");
        updated.Defender = actionDto.DefenderCardId.HasValue ?
            await _unitOfWork.GameCards.GetByWithCardById(actionDto.DefenderCardId.Value) : null;
        updated.DefenderPlayer = actionDto.DefenderPlayerGameId.HasValue ?
            await _unitOfWork.PlayerGames.GetByWithUserById(actionDto.DefenderPlayerGameId.Value) : null;

        _unitOfWork.Attacks.Update(updated);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<AttackActionDto>(updated);
    }

    public async Task<CardMovementDto> EditCardMovement(Guid actionId, CreateCardMovementDto actionDto)
    {
        var existing = await _unitOfWork.CardMovements.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Card Movement not found");
        var updated = _mapper.Map(actionDto, existing);
        updated.Turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");
        updated.Card = await _unitOfWork.GameCards.GetByWithCardById(actionDto.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found");

        _unitOfWork.CardMovements.Update(updated);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CardMovementDto>(updated);
    }

    public async Task<EffectActivationDto> EditEffectActivation(Guid actionId, CreateEffectActivationDto actionDto)
    {
        var existing = await _unitOfWork.EffectActivations.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Effect Activation not found");
        var updated = _mapper.Map(actionDto, existing);
        updated.Turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");
        updated.Effect = await _unitOfWork.Effects.GetByIdAsync(actionDto.EffectId)
            ?? throw new ObjectNotFoundException("Effect not found");
        updated.SourceCard = await _unitOfWork.GameCards.GetByWithCardById(actionDto.SourceCardId)
            ?? throw new ObjectNotFoundException("Source card not found");

        _unitOfWork.EffectActivations.Update(updated);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<EffectActivationDto>(updated);
    }

    public async Task<GameDto> EditGame(Guid id, EditGameDto gameDto)
    {
        var existing = await _unitOfWork.Games.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Game not found");

        var game = _mapper.Map(gameDto, existing);
        _unitOfWork.Games.Update(game);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<GameDto>(game);
    }

    public async Task<GameCardDto> EditGameCard(Guid cardId, EditGameCardDto gameCardDto)
    {
        var existing = await _unitOfWork.GameCards.GetByIdAsync(cardId)
            ?? throw new ObjectNotFoundException("Game Card not found");

        var updated = _mapper.Map(gameCardDto, existing);
        _unitOfWork.GameCards.Update(updated);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<GameCardDto>(updated);
    }

    public async Task<TurnDto> EditGameTurn(Guid turnId, EditTurnDto turnDto)
    {
        var existing = await _unitOfWork.Turns.GetByIdAsync(turnId)
            ?? throw new ObjectNotFoundException("Turn not found");

        var updated = _mapper.Map(turnDto, existing);
        _unitOfWork.Turns.Update(updated);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<TurnDto>(updated);
    }

    public async Task<PlaceCardDto> EditPlaceAction(Guid actionId, CreatePlaceActionDto actionDto)
    {
        var existing = await _unitOfWork.PlaceCards.GetByIdAsync(actionId)
            ?? throw new ObjectNotFoundException("Place Action not found");
        var updated = _mapper.Map(actionDto, existing);
        updated.Turn = await _unitOfWork.Turns.GetByIdAsync(actionDto.TurnId)
            ?? throw new ObjectNotFoundException("Turn not found");
        updated.Card = await _unitOfWork.GameCards.GetByWithCardById(actionDto.GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found");

        _unitOfWork.PlaceCards.Update(updated);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<PlaceCardDto>(updated);
    }

    public async Task<PlayerGameDto> EditPlayerGame(Guid id, Guid userId, EditPlayerGameDto playerGameDto)
    {
        var pg = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(userId, id)
            ?? throw new ObjectNotFoundException("Player Game not found");

        var updated = _mapper.Map(playerGameDto, pg);
        _unitOfWork.PlayerGames.Update(updated);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<PlayerGameDto>(updated);
    }

    public async Task<AttackActionDto?> GetAttackAction(Guid actionId)
    {
        var action = await _unitOfWork.Attacks.GetByIdWithIncludesAsync(actionId);
        return _mapper.Map<AttackActionDto?>(action);
    }

    public async Task<PagedResult<AttackActionDto>> GetAttackActions(Guid id, int page, int pageSize)
    {
        var actions = await _unitOfWork.Attacks.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(a => a.ExecutedAt), a => a.Turn.GameId == id,
            includeProperties: "Turn,Attacker.Card,Defender.Card,DefenderPlayer.User"
        );
        return _mapper.Map<PagedResult<AttackActionDto>>(actions);
    }

    public async Task<CardMovementDto?> GetCardMovement(Guid actionId)
    {
        var action = await _unitOfWork.CardMovements.GetByIdWithIncludesAsync(actionId);
        return _mapper.Map<CardMovementDto?>(action);
    }

    public async Task<PagedResult<CardMovementDto>> GetCardMovements(Guid id, int page, int pageSize)
    {
        var actions = await _unitOfWork.CardMovements.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(a => a.ExecutedAt), a => a.Turn.GameId == id,
            includeProperties: "Turn,Card.Card");
        return _mapper.Map<PagedResult<CardMovementDto>>(actions);
    }

    public async Task<EffectActivationDto?> GetEffectActivation(Guid actionId)
    {
        var action = await _unitOfWork.EffectActivations.GetByIdWithIncludesAsync(actionId);
        return _mapper.Map<EffectActivationDto?>(action);
    }

    public async Task<PagedResult<EffectActivationDto>> GetEffectActivations(Guid id, int page, int pageSize)
    {
        var actions = await _unitOfWork.EffectActivations.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(a => a.ActivatedAt), a => a.Turn.GameId == id,
            includeProperties: "Turn,SourceCard.Card"
        );
        return _mapper.Map<PagedResult<EffectActivationDto>>(actions);
    }

    public async Task<GameDto?> GetGameById(Guid id)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(id);
        return _mapper.Map<GameDto?>(game);
    }

    public async Task<GameCardDto?> GetGameCard(Guid cardId)
    {
        var card = await _unitOfWork.GameCards.GetByWithCardById(cardId);
        return _mapper.Map<GameCardDto?>(card);
    }

    public async Task<List<GameCardDto>> GetGameCards(Guid id)
    {
        var cards = await _unitOfWork.GameCards.GetByGameIdWithCardAsync(id);
        return _mapper.Map<List<GameCardDto>>(cards);
    }

    public async Task<GameStateDto> GetGameState(Guid gameId, Guid userId)
    {
        var state = await _gameEngine.GetGameStateAsync(gameId, userId);

        return new GameStateDto
        {
            GameId = state.Game.Id,
            ViewerPlayerId = state.Viewer.Id,
            CurrentTurn = _mapper.Map<TurnDto>(state.CurrentTurn),
            Players = state.Players.Select(player => _mapper.Map<PlayerGameDto>(player)).ToList(),
            Cards = state.Cards.Select(card => MapCardForViewer(card, state.Viewer.Id)).ToList(),
        };
    }

    public async Task<PlayerGameDto> GetGamePlayer(Guid id, Guid userId)
    {
        var pg = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(userId, id)
            ?? throw new ObjectNotFoundException("Player Game not found");
        return _mapper.Map<PlayerGameDto>(pg);
    }

    public async Task<List<PlayerGameDto>> GetGamePlayers(Guid id)
    {
        var players = await _unitOfWork.PlayerGames.GetByGameIdWithUserAsync(id);
        return _mapper.Map<List<PlayerGameDto>>(players);
    }

    public async Task<PagedResult<GameDto>> GetGames(int page, int pageSize)
    {
        var games = await _unitOfWork.Games.GetPagedAsync(page, pageSize, q => q.OrderByDescending(g => g.StartedAt));
        return _mapper.Map<PagedResult<GameDto>>(games);
    }

    public async Task<TurnDto?> GetGameTurn(Guid turnId)
    {
        var turn = await _unitOfWork.Turns.GetByIdAsync(turnId);
        return _mapper.Map<TurnDto?>(turn);
    }

    public async Task<PagedResult<TurnDto>> GetGameTurns(Guid id, int page, int pageSize)
    {
        var turns = await _unitOfWork.Turns.GetPagedAsync(page, pageSize, q => q.OrderBy(t => t.TurnNumber), t => t.GameId == id);
        return _mapper.Map<PagedResult<TurnDto>>(turns);
    }

    public async Task<PlaceCardDto?> GetPlaceAction(Guid actionId)
    {
        var action = await _unitOfWork.PlaceCards.GetByIdWithIncludesAsync(actionId);
        return _mapper.Map<PlaceCardDto?>(action);
    }

    public async Task<PagedResult<PlaceCardDto>> GetPlaceActions(Guid id, int page, int pageSize)
    {
        var actions = await _unitOfWork.PlaceCards.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(a => a.ExecutedAt), a => a.Turn.GameId == id,
            includeProperties: "Turn,Card.Card"
        );
        return _mapper.Map<PagedResult<PlaceCardDto>>(actions);
    }

    public Task<bool> UserExistsInGame(Guid gameId, Guid userId)
    {
        return _gameEngine.UserExistsInGameAsync(gameId, userId);
    }

    private GameCardDto MapCardForViewer(GameCard card, Guid viewerPlayerGameId)
    {
        var isOwner = card.PlayerGameId == viewerPlayerGameId;
        var shouldHideCardData =
            (card.Zone == CardZone.Hand && !isOwner) ||
            card.Zone == CardZone.Deck ||
            (card.Zone == CardZone.Field && card.IsFaceDown && !isOwner);

        return new GameCardDto
        {
            Id = card.Id,
            Zone = card.Zone,
            DeckOrder = card.DeckOrder,
            IsFaceDown = card.IsFaceDown,
            FieldIndex = card.FieldIndex,
            DefensePosition = card.DefensePosition,
            PlayerGameId = card.PlayerGameId,
            Card = shouldHideCardData ? null : _mapper.Map<CardDto>(card.Card),
        };
    }
}
