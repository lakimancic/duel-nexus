using Backend.Application.DTOs.Cards;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface ICardService
{
    public Task<PagedResult<CardDto>> GetCards(int page, int pageSize, string? search);

    public Task<CardDto?> GetCardById(Guid id);

    public Task<CardDto> CreateCard(CreateCardDto cardDto);

    public Task DeleteCard(Guid cardId);

    public Task<CardDto?> EditCard(Guid id, CreateCardDto cardDto);
}
