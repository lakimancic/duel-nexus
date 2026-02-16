using Backend.Application.DTOs.Cards;
using Backend.Application.DTOs.Decks;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IDeckService
{
    Task<PagedResult<DeckDto>> GetDecks(int page, int pageSize, string? search);
    Task<DeckDto?> GetDeckWithUser(Guid id);
    Task<List<DeckDto>?> GetDeckByUserId(Guid id);
    Task<List<DeckDto>> GetCompleteDecksByUserId(Guid id);
    Task<List<DeckDto>> GetDecksByUserId(Guid id);
    Task<DeckDto> CreateDeckForUser(Guid userId, string name);
    Task<DeckDto> CreateDeck(CreateDeckDto deck);
    Task DeleteDeck(Guid id);
    Task AddCards(Guid deckId, List<InsertDeckCardDto> cards);
    Task AddCardForUser(Guid userId, Guid deckId, Guid cardId, int quantity);
    Task RemoveCards(Guid id,List<Guid> cardIds);
    Task RemoveCardForUser(Guid userId, Guid deckId, Guid cardId, int quantity);
    Task<DeckDto> EditDeck(Guid id, EditDeckDto deck);
    Task<List<DeckCardDto>> GetDeckCards(Guid id);
    Task<List<DeckCardDto>> GetDeckCardsByUser(Guid userId, Guid deckId);
}
