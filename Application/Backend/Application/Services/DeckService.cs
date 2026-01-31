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
    public async Task AddCards(Guid deckId, List<InsertDeckCardDto> cards)
    {
        var deck = await GetDeckById(deckId)
            ?? throw new Exception("Deck not found");

        var playerCards = await _unitOfWork.PlayerCards.GetCardsByDeckId(deckId);

        if (playerCards.Count == 0)
            return;

        var existingDeckCards = await _unitOfWork.DeckCardRepository.GetByDeckId(deckId);

        var deckCardsToAdd = playerCards
            .Join(
                cards,
                playerCard => playerCard.Id,                 
                dto => dto.PlayerCardId,
                (playerCard,dto) =>
                {
                    var alreadyInDeck = existingDeckCards
                        .FirstOrDefault(dc => dc.CardId == playerCard.Id)
                        ?.Quantity ?? 0;

                    var allowed = playerCard.Quantity - alreadyInDeck;

                    var quantityToAdd = Math.Min(dto.Quantity, allowed);

                    return new DeckCard
                    {
                        DeckId = deckId,
                        CardId = playerCard.Id,
                        Quantity = Math.Max(quantityToAdd, 0)
                    };
                })
            .Where(dc => dc.Quantity > 0)
            .ToList();

        if (deckCardsToAdd.Count == 0)
            return;

        await _unitOfWork.DeckCardRepository
            .AddCardsInDeckAsync(deckId, deckCardsToAdd);

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
