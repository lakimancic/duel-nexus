using System.Linq.Expressions;
using AutoMapper;
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

    public async Task<List<DeckDto>?> GetDeckByUserId(Guid userId)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        return _mapper.Map<List<Deck>, List<DeckDto>>(decks);
    }

    public async Task<List<DeckDto>> GetCompleteDecksByUserId(Guid userId)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        var completeDecks = decks.Where(deck => deck.IsComplete).ToList();
        return _mapper.Map<List<DeckDto>>(completeDecks);
    }

    public async Task<List<DeckDto>> GetDecksByUserId(Guid userId)
    {
        var decks = await _unitOfWork.Decks.GetByUserId(userId);
        return _mapper.Map<List<DeckDto>>(decks);
    }

    public async Task<DeckDto> CreateDeck(CreateDeckDto deck)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(deck.UserId)
            ?? throw new ObjectNotFoundException("User not found");

        var deckEntity = _mapper.Map<Deck>(deck);
        deckEntity.User = user;
        await _unitOfWork.Decks.AddAsync(deckEntity);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<DeckDto>(deckEntity);
    }

    public async Task<DeckDto> CreateDeckForUser(Guid userId, string name)
    {
        var normalizedName = name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new BadRequestException("Deck name is required.");

        var existingDecks = await _unitOfWork.Decks.GetByUserId(userId);
        if (existingDecks.Count >= GameConstants.MaxDecksPerUser)
            throw new BadRequestException($"Deck limit reached ({GameConstants.MaxDecksPerUser}).");

        if (existingDecks.Any(deck => string.Equals(deck.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
            throw new BadRequestException("Deck with this name already exists.");

        var createdDeck = await CreateDeck(new CreateDeckDto
        {
            UserId = userId,
            Name = normalizedName,
            IsComplete = false
        });

        return createdDeck;
    }

    public async Task AddCards(Guid deckId, List<InsertDeckCardDto> cards)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new ObjectNotFoundException("Deck not found");

        var playerCards = await _unitOfWork.PlayerCards.GetCardsByUserId(deck.UserId);

        if (playerCards.Count == 0)
            return;

        var existingDeckCards = await _unitOfWork.DeckCards.GetByDeckId(deckId);
        var currentDeckSize = existingDeckCards.Sum(deckCard => deckCard.Quantity);
        var fullQuantity = cards.Sum(cd => cd.Quantity);
        if (currentDeckSize + fullQuantity > GameConstants.MaxDeckSize)
            throw new BadRequestException($"Cannot add {fullQuantity} cards, deck size limit is {GameConstants.MaxDeckSize}");

        var deckCardsToAdd = playerCards
            .Join(
                cards,
                playerCard => playerCard.CardId,
                dto => dto.CardId,
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

        var updatedDeckSize = currentDeckSize + deckCardsToAdd.Sum(deckCard => deckCard.Quantity);
        deck.IsComplete = updatedDeckSize == GameConstants.MaxDeckSize;

        await _unitOfWork.CompleteAsync();
    }

    public async Task AddCardForUser(Guid userId, Guid deckId, Guid cardId, int quantity)
    {
        if (quantity <= 0)
            throw new BadRequestException("Quantity must be greater than 0.");

        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new ObjectNotFoundException("Deck not found");

        if (deck.UserId != userId)
            throw new BadRequestException("You can only edit your own deck.");

        var ownedCard = await _unitOfWork.PlayerCards.GetPlayerCard(userId, cardId)
            ?? throw new ObjectNotFoundException("You do not own this card.");

        var deckCards = await _unitOfWork.DeckCards.GetByDeckId(deckId);
        var deckSize = deckCards.Sum(deckCard => deckCard.Quantity);
        if (deckSize >= GameConstants.MaxDeckSize)
            throw new BadRequestException("Deck is already full.");

        var deckCard = await _unitOfWork.DeckCards.GetByDeckAndCardId(deckId, cardId);
        var alreadyInDeck = deckCard?.Quantity ?? 0;
        var availableQuantity = ownedCard.Quantity - alreadyInDeck;
        if (availableQuantity <= 0)
            throw new BadRequestException("No available copies left to add.");

        var quantityToAdd = Math.Min(quantity, availableQuantity);
        quantityToAdd = Math.Min(quantityToAdd, GameConstants.MaxDeckSize - deckSize);
        if (quantityToAdd <= 0)
            throw new BadRequestException("Cannot add card to deck.");

        if (deckCard == null)
        {
            await _unitOfWork.DeckCards.AddAsync(new DeckCard
            {
                DeckId = deckId,
                CardId = cardId,
                Quantity = quantityToAdd
            });
        }
        else
        {
            deckCard.Quantity += quantityToAdd;
            _unitOfWork.DeckCards.Update(deckCard);
        }

        deck.IsComplete = deckSize + quantityToAdd == GameConstants.MaxDeckSize;
        _unitOfWork.Decks.Update(deck);
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
        var deck = await _unitOfWork.Decks.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Deck not found");
        var cards = await _unitOfWork.DeckCards.GetByDeckId(id);
        deck.IsComplete = cards.Sum(card => card.Quantity) == GameConstants.MaxDeckSize;
        _unitOfWork.Decks.Update(deck);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveCardForUser(Guid userId, Guid deckId, Guid cardId, int quantity)
    {
        if (quantity <= 0)
            throw new BadRequestException("Quantity must be greater than 0.");

        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new ObjectNotFoundException("Deck not found");

        if (deck.UserId != userId)
            throw new BadRequestException("You can only edit your own deck.");

        var deckCard = await _unitOfWork.DeckCards.GetByDeckAndCardId(deckId, cardId)
            ?? throw new ObjectNotFoundException("Card is not in deck.");

        if (quantity >= deckCard.Quantity)
            _unitOfWork.DeckCards.Delete(deckCard);
        else
        {
            deckCard.Quantity -= quantity;
            _unitOfWork.DeckCards.Update(deckCard);
        }

        var cards = await _unitOfWork.DeckCards.GetByDeckId(deckId);
        var totalQuantity = cards.Sum(card => card.Quantity);
        deck.IsComplete = totalQuantity == GameConstants.MaxDeckSize;
        _unitOfWork.Decks.Update(deck);
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

    public async Task<List<DeckCardDto>> GetDeckCardsByUser(Guid userId, Guid deckId)
    {
        var deck = await _unitOfWork.Decks.GetByIdAsync(deckId)
            ?? throw new ObjectNotFoundException("Deck not found");

        if (deck.UserId != userId)
            throw new BadRequestException("You can only access your own deck.");

        return await GetDeckCards(deckId);
    }
}
