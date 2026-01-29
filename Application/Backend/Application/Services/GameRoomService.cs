using Backend.Application.DTOs.GameRooms;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using AutoMapper;
namespace Backend.Application.Services;

public class GameRoomService(IUnitOfWork unitOfWork, IMapper mapper) : IGameRoomService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<GameRoomDto> CreateGameRoomAsync(Guid userId, Guid deckId)
    {
        var host = await _unitOfWork.Users.GetByIdAsync(userId) ?? throw new NotFoundException("User not found");
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId) ?? throw new NotFoundException("Deck not found");
        var gameRoom = await _unitOfWork.GameRooms.CreateGameRoomByHostAsync(host, deck);
        await _unitOfWork.CompleteAsync();

        var created = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoom.Id);
        return _mapper.Map<GameRoom, GameRoomDto>(created!);
    }

    public async Task<IEnumerable<GameRoomDto>> GetAllGameRoomsAsync()
    {
        var rooms = await _unitOfWork.GameRooms.GetAllWithPlayersAsync();
        return _mapper.Map<List<GameRoom>, List<GameRoomDto>>(rooms);
    }

    public async Task<GameRoomDto?> GetGameRoomByIdAsync(Guid gameRoomId)
    {
        var room = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoomId);
        return room == null ? null : _mapper.Map<GameRoom, GameRoomDto>(room);
    }

    public async Task<Guid> JoinGameRoomAsync(string code, Guid userId, Guid deckId)
    {
        var gameRoom = await _unitOfWork.GameRooms.GetByCodeAsync(code) ?? throw new NotFoundException("Game room not found");
        var user = await _unitOfWork.Users.GetByIdAsync(userId) ?? throw new NotFoundException("User not found");
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId) ?? throw new NotFoundException("Deck not found");
        await _unitOfWork.GameRoomPlayers.AddAsync(new GameRoomPlayer
        {
            GameRoom = gameRoom,
            User = user,
            Deck = deck,
        });
        await _unitOfWork.CompleteAsync();

        var updated = await _unitOfWork.GameRooms.GetByIdWithPlayersAsync(gameRoom.Id);
        return updated!.Id;
    }

    public async Task LeaveGameRoomAsync(Guid gameRoomId, Guid userId)
    {
        var player = await _unitOfWork.GameRoomPlayers.GetByGameRoomIdAndPlayerIdAsync(gameRoomId, userId)
            ?? throw new NotFoundException("Player not found in the game room");
        _unitOfWork.GameRoomPlayers.Delete(player);
        await _unitOfWork.CompleteAsync();
    }
}
