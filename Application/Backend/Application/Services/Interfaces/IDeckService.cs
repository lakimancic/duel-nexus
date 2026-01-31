using Backend.Application.DTOs.Decks;

namespace Backend.Application.Services.Interfaces;

public interface IDeckService
{
    public Task<List<DeckDto>> GetAllDecks();

    public Task<DeckDto?> GetDeckById(Guid id);
    public Task<List<DeckDto>?> GetDeckByUserId(Guid id);
    public Task<DeckDto> CreateDeck(DeckDto deck);
    public Task DeleteDeck(DeckDto deck);
    public Task AddCards(Guid deckId, List<DeckCardDto> cards);
    public Task RemoveCards(Guid id,List<Guid> cardIds);
}
