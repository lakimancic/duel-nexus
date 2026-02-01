using AutoMapper;
using Backend.Application.DTOs.Decks;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;

namespace Backend.Application.Services;

public class CardService(IUnitOfWork unitOfWork, IMapper mapper) : ICardService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    public async Task<List<CardDto>> GetCardsAsync(int page, int pageSize)
    {
        List<Card> cards =  (await _unitOfWork.Cards.GetPagedAsync(page, pageSize)).Items;
        return _mapper.Map<List<Card>, List<CardDto>>(cards);
    }

    public async Task<CardDto?> GetCardById(Guid id)
    {
        var card = await _unitOfWork.Cards.GetByIdAsync(id);
        if(card == null) return null;
        return _mapper.Map<Card, CardDto>(card);
    }

    public async Task<CardDto> CreateCard(CreateCardDto cardDto)
    {
        if(cardDto.Type != CardType.Monster)
        {
            cardDto.Attack = 0;
            cardDto.Defense = 0;
            cardDto.Level = 0;
        }

        Card card = _mapper.Map<CreateCardDto, Card>(cardDto);
        await _unitOfWork.Cards.AddAsync(card);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Card, CardDto>(card);
    }

    public async Task DeleteCard(Guid cardId)
    {
        var card = await _unitOfWork.Cards.GetByIdAsync(cardId)
            ?? throw new KeyNotFoundException($"Card with ID '{cardId}' not found");
        _unitOfWork.Cards.Delete(card);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<CardDto?> EditCard(Guid id, CreateCardDto cardDto)
    {
        var existingCard = await _unitOfWork.Cards.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Card with ID '{id}' not found");

        if(cardDto.Type != CardType.Monster)
        {
            cardDto.Attack = 0;
            cardDto.Defense = 0;
            cardDto.Level = 0;
        }

        var card = _mapper.Map(cardDto, existingCard);

        _unitOfWork.Cards.Update(card);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Card, CardDto>(card);
    }

}
