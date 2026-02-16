using AutoMapper;
using Backend.Application.DTOs.GameRooms;
using Backend.Application.DTOs.Games;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;
using Backend.Utils.WebApi;

namespace Backend.Application.Services;

public class GameRoomService(IUnitOfWork unitOfWork, IMapper mapper, IGameService gameService) : IGameRoomService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IGameService _gameService = gameService;

    public async Task<GameRoomPlayerDto> AddPlayerToGameRoom(Guid id, InsertGameRoomPlayerDto playerDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(playerDto.UserId)
            ?? throw new ObjectNotFoundException("User not found");
        var gameRoom = await _unitOfWork.GameRooms.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Game Room not found");

        if (gameRoom.Status != RoomStatus.Waiting)
            throw new BadRequestException("Cannot join this room.");

        var existingPlayer = await _unitOfWork.GameRoomPlayers.GetByRoomAndUserId(id, playerDto.UserId);
        if (existingPlayer != null)
            return _mapper.Map<GameRoomPlayerDto>(existingPlayer);

        var gameRoomPlayer = new GameRoomPlayer
        {
            UserId = playerDto.UserId,
            User = user,
            GameRoomId = id,
            GameRoom = gameRoom
        };
        await _unitOfWork.GameRoomPlayers.AddAsync(gameRoomPlayer);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<GameRoomPlayerDto>(gameRoomPlayer);
    }

    public async Task<GameRoomDto> CreateRoom(CreateGameRoomDto gameRoomDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(gameRoomDto.HostUserId)
            ?? throw new ObjectNotFoundException("User not found");

        var gameRoom = _mapper.Map<GameRoom>(gameRoomDto);
        if (!gameRoom.IsRanked)
            gameRoom.JoinCode = await GenerateUniqueJoinCode();
        gameRoom.HostUser = user;
        await _unitOfWork.GameRooms.AddAsync(gameRoom);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<GameRoomDto>(gameRoom);
    }

    public async Task<GameRoomDto> CreateFriendlyRoom(Guid hostUserId)
    {
        if (!await HasAtLeastOneCompleteDeck(hostUserId))
            throw new BadRequestException("You need at least one complete deck to create a friendly room.");

        var room = await CreateRoom(new CreateGameRoomDto
        {
            HostUserId = hostUserId,
            IsRanked = false,
        });

        await AddPlayerToGameRoom(room.Id, new InsertGameRoomPlayerDto { UserId = hostUserId });
        return room;
    }

    public async Task<GameRoomDto> JoinFriendlyRoom(Guid userId, string joinCode)
    {
        if (!await HasAtLeastOneCompleteDeck(userId))
            throw new BadRequestException("You need at least one complete deck to join a friendly room.");

        var normalizedJoinCode = joinCode.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalizedJoinCode))
            throw new BadRequestException("Join code is required.");

        var room = await _unitOfWork.GameRooms.GetFriendlyWaitingByJoinCode(normalizedJoinCode)
            ?? throw new ObjectNotFoundException("Friendly room not found.");

        var existingPlayer = await _unitOfWork.GameRoomPlayers.GetByRoomAndUserId(room.Id, userId);
        if (existingPlayer == null)
            await AddPlayerToGameRoom(room.Id, new InsertGameRoomPlayerDto { UserId = userId });

        return _mapper.Map<GameRoomDto>(room);
    }

    public async Task DeleteGameRoomPlayer(Guid id, Guid userId)
    {
        var gameRoomPlayer = await _unitOfWork.GameRoomPlayers.GetByRoomAndUserId(id, userId)
            ?? throw new ObjectNotFoundException("Game Room Player not found");

        _unitOfWork.GameRoomPlayers.Delete(gameRoomPlayer);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> LeaveRoom(Guid id, Guid userId)
    {
        var room = await _unitOfWork.GameRooms.GetWithHostById(id)
            ?? throw new ObjectNotFoundException("Game Room not found");

        if (room.Status != RoomStatus.Waiting)
            throw new BadRequestException("Cannot leave this room.");

        if (room.HostUserId == userId)
        {
            await CancelRoom(id, userId);
            return true;
        }

        await DeleteGameRoomPlayer(id, userId);
        return false;
    }

    public async Task CancelRoom(Guid id, Guid hostUserId)
    {
        var room = await _unitOfWork.GameRooms.GetWithHostById(id)
            ?? throw new ObjectNotFoundException("Game Room not found");

        if (room.HostUserId != hostUserId)
            throw new BadRequestException("Only the host can cancel the room.");

        if (room.Status != RoomStatus.Waiting)
            throw new BadRequestException("Only waiting rooms can be canceled.");

        var players = await _unitOfWork.GameRoomPlayers.GetByGameRoomId(id);
        foreach (var player in players)
            _unitOfWork.GameRoomPlayers.Delete(player);

        room.Status = RoomStatus.Cancelled;
        _unitOfWork.GameRooms.Update(room);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<Guid> StartRoomGame(Guid id, Guid hostUserId)
    {
        var room = await _unitOfWork.GameRooms.GetWithHostById(id)
            ?? throw new ObjectNotFoundException("Game Room not found");

        if (room.HostUserId != hostUserId)
            throw new BadRequestException("Only the host can start the game.");

        if (room.Status != RoomStatus.Waiting)
            throw new BadRequestException("Room is not in waiting state.");

        var players = await _unitOfWork.GameRoomPlayers.GetByGameRoomId(id);
        if (players.Count < 2)
            throw new BadRequestException("At least 2 players are required to start.");

        if (players.Any(player => !player.DeckId.HasValue))
            throw new BadRequestException("All players must be ready before starting.");

        room.Status = RoomStatus.Started;
        _unitOfWork.GameRooms.Update(room);

        var game = await _gameService.CreateGame(new CreateGameDto { RoomId = id });
        await _unitOfWork.CompleteAsync();
        return game.Id;
    }

    public async Task<GameRoomDto> EditGameRoom(Guid id, EditGameRoomDto gameRoomDto)
    {
        var existingGameRoom = await _unitOfWork.GameRooms.GetWithHostById(id)
            ?? throw new ObjectNotFoundException("Game Room not found");

        var gameRoom = _mapper.Map(gameRoomDto, existingGameRoom);
        _unitOfWork.GameRooms.Update(gameRoom);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<GameRoomDto>(gameRoom);
    }

    public async Task<GameRoomPlayerDto> SetPlayerGameDeck(Guid id, Guid userId, Guid deckId)
    {
        var room = await _unitOfWork.GameRooms.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Game Room not found");

        if (room.Status != RoomStatus.Waiting)
            throw new BadRequestException("Deck can be changed only while room is waiting.");

        var gameRoomPlayer = await _unitOfWork.GameRoomPlayers.GetByRoomAndUserId(id, userId)
            ?? throw new ObjectNotFoundException("Game Room Player not found");
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new ObjectNotFoundException("Deck not found");

        if (deck.UserId != userId)
            throw new BadRequestException("You can only choose your own deck.");

        if (!deck.IsComplete)
            throw new BadRequestException("Only complete decks can be selected.");

        gameRoomPlayer.DeckId = deckId;
        gameRoomPlayer.Deck = deck;
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<GameRoomPlayerDto>(gameRoomPlayer);
    }

    public async Task<GameRoomDto?> GetGameRoomById(Guid id)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetWithHostById(id);
        return _mapper.Map<GameRoomDto?>(gameRoom);
    }

    public async Task<List<GameRoomPlayerDto>> GetGameRoomPlayers(Guid id)
    {
        var players = await _unitOfWork.GameRoomPlayers.GetByGameRoomId(id);
        return _mapper.Map<List<GameRoomPlayerDto>>(players);
    }

    public async Task<PagedResult<GameRoomDto>> GetRooms(int page, int pageSize)
    {
        var rooms = await _unitOfWork.GameRooms.GetPagedAsync(
            page, pageSize, q => q.OrderByDescending(gr => gr.CreatedAt), includeProperties: "HostUser"
        );
        return _mapper.Map<PagedResult<GameRoomDto>>(rooms);
    }

    public async Task<bool> UserExistsInRoom(Guid id, Guid userId)
    {
        var gameRoomPlayer = await _unitOfWork.GameRoomPlayers.GetByRoomAndUserId(id, userId);
        return gameRoomPlayer != null;
    }

    public async Task<bool> HasAtLeastOneCompleteDeck(Guid userId)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        return decks.Any(deck => deck.IsComplete);
    }

    private async Task<string> GenerateUniqueJoinCode()
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var joinCode = Guid.NewGuid().ToString("N")[0..6].ToUpperInvariant();
            var existing = await _unitOfWork.GameRooms.GetFriendlyWaitingByJoinCode(joinCode);
            if (existing == null)
                return joinCode;
        }

        throw new BadRequestException("Could not generate join code. Please try again.");
    }
}
