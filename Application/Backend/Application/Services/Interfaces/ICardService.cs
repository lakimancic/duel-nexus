using Backend.Application.DTOs.Decks;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface ICardService
{
    public Task<PagedResult<CardDto>> GetCardsAsync(int page, int pageSize);

    public Task<CardDto?> GetCardById(Guid id);

    public Task<CardDto> CreateCard(CreateCardDto cardDto);

    public Task DeleteCard(Guid cardId);

    public Task<CardDto?> EditCard(Guid id, CreateCardDto cardDto);
}
