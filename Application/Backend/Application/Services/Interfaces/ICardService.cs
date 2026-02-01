using Backend.Application.DTOs.Decks;

namespace Backend.Application.Services.Interfaces;

public interface ICardService
{
    public Task<List<CardDto>> GetCardsAsync(int page, int pageSize);

    public Task<CardDto?> GetCardById(Guid id);

    public Task<CardDto> CreateCard(CreateCardDto cardDto);

    public Task DeleteCard(Guid cardId);

    public Task<CardDto?> EditCard(Guid id, CreateCardDto cardDto);
}
