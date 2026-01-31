using Backend.Application.DTOs.Player;
namespace Backend.Application.Services.Interfaces;

public interface IPlayerCardService
{
    public Task<List<PlayerCardDto>> GetAllPlayerCards();

    public Task<PlayerCardDto?> GetPlayerCardById(Guid id);

    public Task<List<PlayerCardDto>?> GetPlayerCardsByDeckId(Guid deckId);

    public Task<PlayerCardDto> CreatePlayerCard(CreatePlayerCardDto playerCard);
    public Task DeletePlayerCard(Guid id);
}
