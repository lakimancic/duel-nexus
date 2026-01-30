using Backend.Application.DTOs.GameRooms;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using AutoMapper;

namespace Backend.Application.Services;

public class GameRoomService(IUnitOfWork unitOfWork, IMapper mapper) : IGameRoomService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private const int MaxPlayersPerRoom = 5;

    private void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty");
        if (code.Length != 6)
            throw new ArgumentException("Code must be exactly 6 characters");
    }

    public async Task<GameRoomDto> CreateGameRoomAsync(Guid userId, Guid deckId)
    {
        var host = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID '{userId}' not found");

        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new KeyNotFoundException($"Deck with ID '{deckId}' not found");

        if (deck.UserId != userId)
            throw new InvalidOperationException("Deck does not belong to the user");

        var gameRoom = await _unitOfWork.GameRooms.CreateGameRoomByHostAsync(host, deck);
        await _unitOfWork.CompleteAsync();

        var created = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoom.Id)
            ?? throw new InvalidOperationException("Failed to retrieve created game room");

        return _mapper.Map<GameRoom, GameRoomDto>(created);
    }

    public async Task<IEnumerable<GameRoomDto>> GetAllGameRoomsAsync()
    {
        var rooms = await _unitOfWork.GameRooms.GetAllWithPlayersAsync();
        return _mapper.Map<List<GameRoom>, List<GameRoomDto>>(rooms);
    }

    public async Task<GameRoomDto?> GetGameRoomByIdAsync(Guid gameRoomId)
    {
        if (gameRoomId == Guid.Empty)
            throw new ArgumentException("GameRoomId cannot be empty");

        var room = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoomId);
        return room == null ? null : _mapper.Map<GameRoom, GameRoomDto>(room);
    }

    public async Task<Guid> JoinGameRoomAsync(string code, Guid userId, Guid deckId)
    {
        ValidateCode(code);

        var gameRoom = await _unitOfWork.GameRooms.GetByCodeAsync(code)
            ?? throw new KeyNotFoundException($"Game room with code '{code}' not found");

        if (gameRoom.Status != RoomStatus.Waiting)
            throw new InvalidOperationException($"Cannot join room with status '{gameRoom.Status}'");

        if (gameRoom.Players.Count >= MaxPlayersPerRoom)
            throw new InvalidOperationException($"Game room is full (max {MaxPlayersPerRoom} players)");

        var playerExists = gameRoom.Players.Any(p => p.UserId == userId);
        if (playerExists)
            throw new InvalidOperationException("Player is already in this game room");

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID '{userId}' not found");

        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new KeyNotFoundException($"Deck with ID '{deckId}' not found");

        if (deck.UserId != userId)
            throw new InvalidOperationException("Deck does not belong to the user");

        await _unitOfWork.GameRoomPlayers.AddAsync(new GameRoomPlayer
        {
            GameRoom = gameRoom,
            User = user,
            Deck = deck,
        });
        await _unitOfWork.CompleteAsync();

        var updated = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoom.Id)
            ?? throw new InvalidOperationException("Failed to retrieve updated game room");

        return updated.Id;
    }

    public async Task LeaveGameRoomAsync(Guid gameRoomId, Guid userId)
    {
        if (gameRoomId == Guid.Empty)
            throw new ArgumentException("GameRoomId cannot be empty");

        var gameRoom = await _unitOfWork.GameRooms.GetByIdAsync(gameRoomId)
            ?? throw new KeyNotFoundException($"Game room with ID '{gameRoomId}' not found");

        var player = await _unitOfWork.GameRoomPlayers.GetByGameRoomIdAndPlayerIdAsync(gameRoomId, userId)
            ?? throw new KeyNotFoundException("Player is not in this game room");

        if (gameRoom.HostUserId == userId && gameRoom.Players.Count > 1)
            throw new InvalidOperationException("Host cannot leave the room while other players are present");

        _unitOfWork.GameRoomPlayers.Delete(player);

        if (gameRoom.Players.Count == 1)
        {
            _unitOfWork.GameRooms.Delete(gameRoom);
        }

        await _unitOfWork.CompleteAsync();
    }
}
