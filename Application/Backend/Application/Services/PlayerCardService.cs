using AutoMapper;
using Backend.Application.DTOs.Player;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;

namespace Backend.Application.Services.Interfaces;

public class PlayerCardService(IUnitOfWork unitOfWork, IMapper mapper) : IPlayerCardService
{

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<PlayerCardDto> CreatePlayerCard(CreatePlayerCardDto playerCard)
    {
        var card = _mapper.Map<CreatePlayerCardDto, PlayerCard>(playerCard);
        await _unitOfWork.PlayerCards.AddAsync(card);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<PlayerCard, PlayerCardDto>(card);
    }

    public async Task DeletePlayerCard(Guid id)
    {
        var card = await _unitOfWork.PlayerCards.GetByIdAsync(id) ?? throw new KeyNotFoundException("PlayerCard not found");
        _unitOfWork.PlayerCards.Delete(card);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<List<PlayerCardDto>> GetAllPlayerCards()
    {
        var cards = await _unitOfWork.PlayerCards.GetAllAsync();
        return [.. cards.Select(c => _mapper.Map<PlayerCard, PlayerCardDto>(c))];
    }

    public async Task<PlayerCardDto?> GetPlayerCardById(Guid id)
    {
        var card = await _unitOfWork.PlayerCards.GetByIdAsync(id);
        return card == null ? null : _mapper.Map<PlayerCard, PlayerCardDto>(card);
    }

    public async Task<List<PlayerCardDto>?> GetPlayerCardsByDeckId(Guid deckId)
    {
        var cards = await _unitOfWork.PlayerCards.GetCardsByDeckId(deckId);
        return [.. cards.Select(c => _mapper.Map<PlayerCard, PlayerCardDto>(c))];
    }
}
