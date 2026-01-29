using AutoMapper;
using Backend.Application.DTOs.Decks;
using Backend.Application.Services.Interfaces;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;

namespace Backend.Application.Services;

public class CardService(IUnitOfWork unitOfWork, IMapper mapper) : ICardService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    public async Task<List<CardDto>> GetAllCards()
    {
        List<Card> cards =  await _unitOfWork.Cards.GetAllAsync();
        return _mapper.Map<List<Card>, List<CardDto>>(cards);
    }

    public async Task<CardDto?> GetCardById(Guid id)
    {
        var card = await _unitOfWork.Cards.GetByIdAsync(id);
        if(card == null) return null;
        return _mapper.Map<Card, CardDto>(card);
    }

    public async Task<CardDto> CreateCard(CardDto cardDto)
    {
        Card card = _mapper.Map<CardDto, Card>(cardDto);
        await _unitOfWork.Cards.AddAsync(card);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Card, CardDto>(card);
    }

    public async Task DeleteCard(Guid cardId)
    {
        var card = await _unitOfWork.Cards.GetByIdAsync(cardId);
        if (card == null) return;
        _unitOfWork.Cards.Delete(card);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<CardDto?> EditCard(Guid id, CardDto cardDto)
    {
        var existingCard = await _unitOfWork.Cards.GetByIdAsync(id);
        if (existingCard == null) return null;

        var card = _mapper.Map(cardDto, existingCard);

        _unitOfWork.Cards.Update(card);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Card, CardDto>(card);
    }

}
