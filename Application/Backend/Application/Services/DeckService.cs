using System.Linq.Expressions;
using AutoMapper;
using Backend.Application.DTOs.Cards;
using Backend.Application.DTOs.Decks;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Domain;
using Backend.Utils.Data;
using Backend.Utils.WebApi;

namespace Backend.Application.Services;

public class DeckService(IUnitOfWork unitOfWork, IMapper mapper) : IDeckService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<DeckDto?> GetDeckById(Guid id)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(id);
        return _mapper.Map<DeckDto?>(deck);
    }

    public async Task<List<DeckDto>?> GetDeckByUserId(Guid userId)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        return _mapper.Map<List<Deck>, List<DeckDto>>(decks);
    }

    public async Task<DeckDto> CreateDeck(CreateDeckDto deck)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(deck.UserId)
            ?? throw new KeyNotFoundException("User not found");

        var deckEntity = _mapper.Map<Deck>(deck);
        deckEntity.User = user;
        await _unitOfWork.Decks.AddAsync(deckEntity);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<DeckDto>(deckEntity);
    }

    public async Task AddCards(Guid deckId, List<InsertDeckCardDto> cards)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new KeyNotFoundException("Deck not found");

        var playerCards = await _unitOfWork.PlayerCards.GetCardsByUserId(deck.UserId);

        if (playerCards.Count == 0)
            return;

        var existingDeckCards = await _unitOfWork.DeckCards.GetByDeckId(deckId);
        var fullQuantity = cards.Sum(cd => cd.Quantity);
        if (existingDeckCards.Count + fullQuantity > GameConstants.MaxDeckSize)
            throw new BadRequestException($"Cannot add {fullQuantity} cards, deck size limit is {GameConstants.MaxDeckSize}");

        var deckCardsToAdd = playerCards
            .Join(
                cards,
                playerCard => playerCard.Id,
                dto => dto.PlayerCardId,
                (playerCard,dto) =>
                {
                    var alreadyInDeck = existingDeckCards
                        .FirstOrDefault(dc => dc.CardId == playerCard.CardId)
                        ?.Quantity ?? 0;

                    var allowed = playerCard.Quantity - alreadyInDeck;
                    var quantityToAdd = Math.Min(dto.Quantity, allowed);

                    return new DeckCard
                    {
                        DeckId = deckId,
                        CardId = playerCard.CardId,
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
        }

        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteDeck(Guid id)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Deck not found");
        _unitOfWork.Decks.Delete(deck);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveCards(Guid id,List<Guid> cardIds)
    {
        await _unitOfWork.DeckCards.DeleteManyCardAsync(id,cardIds);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<PagedResult<DeckDto>> GetDecks(int page, int pageSize, string? search)
    {
        Expression<Func<Deck, bool>>? filter = null;
        if (search != null)
            filter = u => u.Name.Contains(search);
        var decks = await _unitOfWork.Decks.GetPagedAsync(
            page, pageSize, q => q.OrderBy(d => d.Id), filter, "User"
        );
        return _mapper.Map<PagedResult<DeckDto>>(decks);
    }

    public async Task<DeckDto?> GetDeckWithUser(Guid id)
    {
        var deck = await _unitOfWork.Decks.GetDeckWithUser(id);
        return _mapper.Map<DeckDto?>(deck);
    }

    public async Task<DeckDto> EditDeck(Guid id, EditDeckDto deckDto)
    {
        var existingDeck = await _unitOfWork.Decks.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Deck not found");

        var deck = _mapper.Map(deckDto, existingDeck);
        _unitOfWork.Decks.Update(deck);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<DeckDto>(deck);
    }

    public async Task<List<DeckCardDto>> GetDeckCards(Guid id)
    {
        var cards = await _unitOfWork.DeckCards.GetByDeckId(id);
        return _mapper.Map<List<DeckCardDto>>(cards);
    }
}
