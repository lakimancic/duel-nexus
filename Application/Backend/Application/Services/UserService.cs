using System.Linq.Expressions;
using AutoMapper;
using Backend.Application.DTOs.Users;
using Backend.Application.DTOs.Auth;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;
using Backend.Utils.Security;
using Backend.Utils.WebApi;
using Backend.Application.DTOs.Decks;

namespace Backend.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, PasswordHasher passwordHasher) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly PasswordHasher _passwordHasher = passwordHasher;

    public async Task<bool> ExistsAsync(string email)
    {
        return await _unitOfWork.Users.ExistsAsync(email);
    }

    public async Task<UserDto?> GetUserById(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return _mapper.Map<UserDto?>(user);
    }

    public async Task<PagedResult<UserDto>> GetUsers(int page, int pageSize, string? search)
    {
        Expression<Func<User, bool>>? filter = null;
        if (search != null)
            filter = u => u.Username.Contains(search);
        var users = await _unitOfWork.Users.GetPagedAsync(
            page, pageSize, q => q.OrderBy(u => u.Id), filter
        );
        return _mapper.Map<PagedResult<UserDto>>(users);
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
        if (user == null || !_passwordHasher.Verify(loginDto.Password, user.Password))
            return null;
        return _mapper.Map<User, UserDto>(user);
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        User user = _mapper.Map<RegisterDto, User>(registerDto);
        var passwordHash = _passwordHasher.Hash(user.Password);
        user.Password = passwordHash;
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<User, UserDto>(user);
    }

    public async Task<UserDto> EditUser(Guid id, EditUserDto userDto)
    {
        var existingUser = await _unitOfWork.Users.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("User not found");

        var user = _mapper.Map(userDto, existingUser);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("User not found");

        _unitOfWork.Users.Delete(user);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PagedResult<PlayerCardDto>> GetPlayerCards(Guid id, int page, int pageSize, string? search)
    {
        Expression<Func<PlayerCard, bool>>? filter = null;
        if (search != null)
            filter = u => u.Card.Name.Contains(search) && u.UserId == id;
        var cards = await _unitOfWork.PlayerCards.GetPagedAsync(
            page, pageSize, q => q.OrderBy(u => u.Id), filter, "Card"
        );
        return _mapper.Map<PagedResult<PlayerCardDto>>(cards);
    }

    public async Task<PlayerCardDto> CreatePlayerCard(Guid id, CreatePlayerCardDto playerCardDto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("User not found");
        var card = await _unitOfWork.Cards.GetByIdAsync(playerCardDto.CardId)
            ?? throw new ObjectNotFoundException("Card not found");

        var playerCard = _mapper.Map<CreatePlayerCardDto, PlayerCard>(playerCardDto);
        playerCard.Card = card;
        playerCard.UserId = id;
        playerCard.User = user;
        await _unitOfWork.PlayerCards.AddAsync(playerCard);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<PlayerCard, PlayerCardDto>(playerCard);
    }

    public async Task DeletePlayerCard(Guid userId, Guid cardId)
    {
        var playerCard = await _unitOfWork.PlayerCards.GetPlayerCard(userId, cardId)
            ?? throw new ObjectNotFoundException("Player Card not found");
        _unitOfWork.PlayerCards.Delete(playerCard);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PlayerCardDto?> EditPlayerCard(Guid userId, Guid cardId, EditPlayerCardDto editPlayerCard)
    {
        var playerCard = await _unitOfWork.PlayerCards.GetPlayerCard(userId, cardId)
            ?? throw new ObjectNotFoundException("Player Card not found");
        PlayerCardDto? result = null;
        if (editPlayerCard.Quantity == 0)
            _unitOfWork.PlayerCards.Delete(playerCard);
        else
        {
            playerCard.Quantity = editPlayerCard.Quantity;
            result = _mapper.Map<PlayerCardDto>(playerCard);
        }
        await _unitOfWork.CompleteAsync();
        return result;

    }

    public async Task<List<DeckDto>> GetPlayerDecks(Guid id)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(id);
        return _mapper.Map<List<DeckDto>>(decks);
    }

    public async Task<ShortUserDto?> GetShortUserById(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return _mapper.Map<ShortUserDto?>(user);
    }
}
