using AutoMapper;
using Backend.Application.DTOs.Decks;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;

namespace Backend.Application.Services;

public class DeckService(IUnitOfWork unitOfWork, IMapper mapper) : IDeckService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<List<DeckDto>> GetAllDecks()
    {
        List<Deck> decks = await _unitOfWork.Decks.GetAllAsync();
        return _mapper.Map<List<Deck>, List<DeckDto>>(decks);
    }

    public async Task<DeckDto?> GetDeckById(Guid id)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(id);
        if(deck == null) return null;
        return _mapper.Map<Deck,DeckDto>(deck);
    }

    public async Task<List<DeckDto>?> GetDeckByUserId(Guid userId)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        return _mapper.Map<List<Deck>, List<DeckDto>>(decks);
    }

    public async Task<DeckDto> CreateDeck(DeckDto deck)
    {
        var deckEntity = _mapper.Map<DeckDto, Deck>(deck);
        await _unitOfWork.Decks.AddAsync(deckEntity);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Deck, DeckDto>(deckEntity);
    }

    public async Task AddCards(Guid deckId, List<DeckCardDto> cards)
    {
        var _ = await GetDeckById(deckId) ?? throw new Exception("Deck not found");

        List<DeckCard> cardsList = _mapper.Map<List<DeckCardDto>, List<DeckCard>>(cards);
        await _unitOfWork.DeckCardRepository.AddCardsInDeckAsync(deckId, cardsList);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteDeck(DeckDto deck)
    {
        var deckEntity = _mapper.Map<DeckDto, Deck>(deck);
        _unitOfWork.Decks.Delete(deckEntity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveCards(Guid id,List<Guid> cardIds)
    {
        await _unitOfWork.DeckCardRepository.DeleteManyCardAsync(id,cardIds);
        await _unitOfWork.CompleteAsync();
    }
}
