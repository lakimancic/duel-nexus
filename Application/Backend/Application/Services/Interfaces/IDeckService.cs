using Backend.Application.DTOs.Decks;
using Backend.Utils.Data;

namespace Backend.Application.Services.Interfaces;

public interface IDeckService
{
    public Task<PagedResult<DeckDto>> GetDecksAsync(int page, int pageSize);
    public Task<DeckDto?> GetDeckById(Guid id);
    public Task<List<DeckDto>?> GetDeckByUserId(Guid id);
    public Task<DeckDto> CreateDeck(DeckDto deck);
    public Task DeleteDeck(DeckDto deck);
    public Task AddCards(Guid deckId, List<InsertDeckCardDto> cards);
    public Task RemoveCards(Guid id,List<Guid> cardIds);
}
