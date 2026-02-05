using Backend.Application.DTOs.Cards;
using Backend.Application.DTOs.Decks;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IDeckService
{
    Task<PagedResult<DeckDto>> GetDecks(int page, int pageSize, string? search);
    Task<DeckDto?> GetDeckById(Guid id);
    Task<DeckDto?> GetDeckWithUser(Guid id);
    Task<List<DeckDto>?> GetDeckByUserId(Guid id);
    Task<DeckDto> CreateDeck(CreateDeckDto deck);
    Task DeleteDeck(Guid id);
    Task AddCards(Guid deckId, List<InsertDeckCardDto> cards);
    Task RemoveCards(Guid id,List<Guid> cardIds);
    Task<DeckDto> EditDeck(Guid id, EditDeckDto deck);
    Task<List<DeckCardDto>> GetDeckCards(Guid id);
}
