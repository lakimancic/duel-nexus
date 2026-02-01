using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using AutoMapper;
using Backend.Application.DTOs.GameRooms;

namespace Backend.Application.Services;

public class GameRoomService(IUnitOfWork unitOfWork, IMapper mapper) : IGameRoomService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private const int MaxPlayersPerRoom = 5;

    public async Task<List<GameRoomDto>> GetGameRoomsAsync(int page, int pageSize, RoomStatus? status)
    {
        if (page <= 0)
            return [];
        var rooms = await _unitOfWork.GameRooms.GetPagedAsync(page, pageSize, gr => status == null || gr.Status == status, includeProperties: "HostUser");
        return [.. rooms.Items.Select(gr => _mapper.Map<GameRoom, GameRoomDto>(gr))];
    }

    public async Task<GameRoomDto> CreateGameRoomAsync(Guid userId)
    {
        var host = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID '{userId}' not found");

        var gameRoom = await _unitOfWork.GameRooms.CreateGameRoomByHostAsync(host);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<GameRoom, GameRoomDto>(gameRoom);
    }

    public async Task<GameRoomDto> GetGameRoomByIdAsync(Guid gameRoomId)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoomId)
            ?? throw new KeyNotFoundException($"Game room with ID '{gameRoomId}' not found");
        return _mapper.Map<GameRoom, GameRoomDto>(gameRoom);
    }

    public async Task AddPlayerToGameRoomAsync(Guid gameRoomId, Guid userId)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetByIdAsync(gameRoomId)
            ?? throw new KeyNotFoundException($"Game room with id '{gameRoomId}' not found");

        if (gameRoom.Status != RoomStatus.Waiting)
            throw new InvalidOperationException($"Cannot join room with status '{gameRoom.Status}'");

        if (gameRoom.Players.Count >= MaxPlayersPerRoom)
            throw new InvalidOperationException($"Game room is full (max {MaxPlayersPerRoom} players)");

        var playerExists = await _unitOfWork.GameRoomPlayers.GetByGameRoomIdAndPlayerIdAsync(gameRoomId, userId) != null;
        if (playerExists)
            throw new InvalidOperationException("Player is already in this game room");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID '{userId}' not found");

        await _unitOfWork.GameRoomPlayers.AddAsync(new GameRoomPlayer
        {
            GameRoom = gameRoom,
            User = user
        });
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemovePlayerFromGameRoomAsync(Guid gameRoomId, Guid userId)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoomId)
            ?? throw new KeyNotFoundException($"Game room with id '{gameRoomId}' not found");

        var player = gameRoom.Players.FirstOrDefault(p => p.UserId == userId)
            ?? throw new KeyNotFoundException("Player not found in this game room");

        _unitOfWork.GameRoomPlayers.Delete(player);
        await _unitOfWork.CompleteAsync();
    }

    public async Task CancelGameRoomAsync(Guid gameRoomId)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetByIdAsync(gameRoomId)
            ?? throw new KeyNotFoundException($"Game room with id '{gameRoomId}' not found");
        gameRoom.Status = RoomStatus.Cancelled;
        _unitOfWork.GameRooms.Update(gameRoom);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdatePlayerDeckAsync(Guid gameRoomId, Guid playerId, Guid? deckId)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoomId)
            ?? throw new KeyNotFoundException($"Game room with id '{gameRoomId}' not found");
        var player = gameRoom.Players.FirstOrDefault(p => p.UserId == playerId)
            ?? throw new KeyNotFoundException("Player not found in this game room");
        var deck = deckId != null
            ? await _unitOfWork.Decks.GetByIdAsync(deckId.Value)
                ?? throw new KeyNotFoundException($"Deck with id '{deckId}' not found")
            : null;
        if (deck != null && deck.UserId != playerId)
            throw new InvalidOperationException("Player does not own the specified deck");

        player.DeckId = deckId;
        _unitOfWork.GameRoomPlayers.Update(player);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<Guid> StartGameFromRoomAsync(Guid gameRoomId)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoomId)
            ?? throw new KeyNotFoundException($"Game room with id '{gameRoomId}' not found");
        if (gameRoom.Status != RoomStatus.Waiting)
            throw new InvalidOperationException($"Cannot start game from room with status '{gameRoom.Status}'");
        if (gameRoom.Players.Count < 2)
            throw new InvalidOperationException("At least two players are required to start the game");

        var game = await _unitOfWork.Games.CreateFromGameRoomAsync(gameRoom);
        gameRoom.Status = RoomStatus.Started;
        _unitOfWork.GameRooms.Update(gameRoom);
        await _unitOfWork.Turns.InitializeTurnsForGameAsync(game);
        await _unitOfWork.CompleteAsync();
        return game.Id;
    }
}
