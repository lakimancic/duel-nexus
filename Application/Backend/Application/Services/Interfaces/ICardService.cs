using Backend.Data.Models;

namespace Backend.Application.Services.Interfaces;

public interface ICardService
{
    public Task<List<CardDto>> GetAllCards();

    public Task<CardDto?> GetCardById(int id);

    public Task<CardDto> CreateCard(CardDto cardDto);
    public Task DeleteCard(CardDto cardDto);

    public Task<CardDto?> EditCard(int id, CardDto cardDto);
}
