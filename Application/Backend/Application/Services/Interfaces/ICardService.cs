using Backend.Data.Models;

namespace Backend.Application.Services.Interfaces;

public interface ICardService
{
    public Task<List<CardDto>> GetAllCards();

    public Task<CardDto?> GetCardById(Guid id);

    public Task<CardDto> CreateCard(CardDto cardDto);
    public Task DeleteCard(Guid cardId);

    public Task<CardDto?> EditCard(Guid id, CardDto cardDto);
}
