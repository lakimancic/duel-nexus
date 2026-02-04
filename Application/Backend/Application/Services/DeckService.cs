using AutoMapper;
using Backend.Application.DTOs.Decks;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Domain;
using Backend.Utils.Data;

namespace Backend.Application.Services;

public class DeckService(IUnitOfWork unitOfWork, IMapper mapper) : IDeckService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

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
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new KeyNotFoundException("Deck not found");

        var playerCards = await _unitOfWork.PlayerCards.GetCardsByDeckId(deckId);

        if (playerCards.Count == 0)
            return;

        var existingDeckCards = await _unitOfWork.DeckCards.GetByDeckId(deckId);
        var fullQuantity = cards.Sum(cd => cd.Quantity);
        if (existingDeckCards.Count + fullQuantity > GameConstants.MaxDeckSize)
            throw new InvalidOperationException($"Cannot add {fullQuantity} cards, deck size limit is {GameConstants.MaxDeckSize}");

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

        await _unitOfWork.DeckCards
            .AddCardsInDeckAsync(deckId, deckCardsToAdd);

        if (existingDeckCards.Count + deckCardsToAdd.Count == GameConstants.MaxDeckSize)
        {
            deck.IsComplete = true;
            _unitOfWork.Decks.Update(deck);
        }

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
        await _unitOfWork.DeckCards.DeleteManyCardAsync(id,cardIds);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PagedResult<DeckDto>> GetDecksAsync(int page, int pageSize)
    {
        var decks = await _unitOfWork.Decks.GetPagedAsync(page, pageSize, q => q.OrderBy(d => d.Id));
        return _mapper.Map<PagedResult<DeckDto>>(decks);
    }
}
