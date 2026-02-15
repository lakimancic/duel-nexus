using Backend.Application.DTOs.Auth;
using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Users;
using Backend.Data.Models;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IUserService
{
    Task<bool> ExistsAsync(string email);
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<UserDto?> LoginAsync(LoginDto loginDto);
    Task<PagedResult<UserDto>> GetUsers(int page, int pageSize, string? search);
    Task<UserDto?> GetUserById(Guid id);
    Task<ShortUserDto?> GetShortUserById(Guid id);
    Task<UserDto> EditUser(Guid id, EditUserDto userDto);
    Task DeleteUser(Guid id);
    Task<PagedResult<PlayerCardDto>> GetPlayerCards(Guid id, int page, int pageSize, string? search);
    Task<PlayerCardDto> CreatePlayerCard(Guid userId, CreatePlayerCardDto playerCardDto);
    Task DeletePlayerCard(Guid userId, Guid cardId);
    Task<PlayerCardDto?> EditPlayerCard(Guid id, Guid cardId, EditPlayerCardDto editPlayerCard);
    Task<List<DeckDto>> GetPlayerDecks(Guid id);
}
