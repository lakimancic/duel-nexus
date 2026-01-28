using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Backend.Data.UnitOfWork;
using Backend.Utils.Security;

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

    public async Task<DeckDto?> GetDeckById(int id)
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

    public async Task<DeckDto> CreateDeck(Deck deck)
    {
        await _unitOfWork.Decks.AddAsync(deck);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Deck, DeckDto>(deck);
    }

    public async Task AddCards(Guid deckId, IEnumerable<(Card card, int quantity)> cardsWithQuantity)
    {
        await _unitOfWork.DeckCardRepository.AddCardsInDeckAsync(deckId, cardsWithQuantity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveCards(Guid deckId, List<Guid> cardIds)
    {
        throw new NotImplementedException();

    }

    public async Task DeleteDeck(Deck deck)
    {
        _unitOfWork.Decks.Delete(deck);
        await _unitOfWork.CompleteAsync();
    }
}