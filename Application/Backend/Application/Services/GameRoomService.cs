using AutoMapper;
using Backend.Application.DTOs.GameRooms;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;
using Backend.Utils.WebApi;

namespace Backend.Application.Services;

public class GameRoomService(IUnitOfWork unitOfWork, IMapper mapper) : IGameRoomService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<GameRoomPlayerDto> AddPlayerToGameRoom(Guid id, InsertGameRoomPlayerDto playerDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(playerDto.UserId)
            ?? throw new ObjectNotFoundException("User not found");
        var gameRoom = await _unitOfWork.GameRooms.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Game Room not found");

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
            gameRoom.JoinCode = Guid.NewGuid().ToString("N")[0..6].ToUpper();
        gameRoom.HostUser = user;
        await _unitOfWork.GameRooms.AddAsync(gameRoom);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<GameRoomDto>(gameRoom);
    }

    public async Task DeleteGameRoomPlayer(Guid id, Guid userId)
    {
        var gameRoomPlayer = await _unitOfWork.GameRoomPlayers.GetByRoomAndUserId(id, userId)
            ?? throw new ObjectNotFoundException("Game Room Player not found");

        _unitOfWork.GameRoomPlayers.Delete(gameRoomPlayer);
        await _unitOfWork.CompleteAsync();
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
        var gameRoomPlayer = await _unitOfWork.GameRoomPlayers.GetByRoomAndUserId(id, userId)
            ?? throw new ObjectNotFoundException("Game Room Player not found");
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new ObjectNotFoundException("Game Room Player not found");

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


}
