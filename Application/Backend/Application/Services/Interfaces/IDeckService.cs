using Backend.Application.DTOs.Auth;
using Backend.Data.Models;

namespace Backend.Application.Services.Interfaces;

public interface IDeckService
{
    public Task<List<DeckDto>> GetAllDecks();

    public Task<DeckDto?> GetDeckById(int id);
    public Task<List<DeckDto>?> GetDeckByUserId(Guid id);
    public Task CreateDeck(Deck deck);
    public Task DeleteDeck(Deck deck);
    public Task AddCards(Guid deckId, IEnumerable<(Card card, int quantity)> cardsWithQuantity);
    public Task RemoveCards(Guid deckId, List<Guid> cardIds);
}
