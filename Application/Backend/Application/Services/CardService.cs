using System.Linq.Expressions;
using AutoMapper;
using Backend.Application.DTOs.Cards;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.Data;
using Backend.Utils.WebApi;

namespace Backend.Application.Services;

public class CardService(IUnitOfWork unitOfWork, IMapper mapper) : ICardService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    public async Task<PagedResult<CardDto>> GetCards(int page, int pageSize, string? search)
    {
        Expression<Func<Card, bool>>? filter = null;
        if (search != null)
            filter = u => u.Name.Contains(search);
        var cards = await _unitOfWork.Cards.GetPagedAsync(
            page, pageSize, q => q.OrderBy(c => c.Id), filter
        );
        return _mapper.Map<PagedResult<CardDto>>(cards);
    }

    public async Task<CardDto?> GetCardById(Guid id)
    {
        var card = await _unitOfWork.Cards.GetCardWithEffectAsync(id);
        return _mapper.Map<CardDto?>(card);
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
            ?? throw new ObjectNotFoundException("Card not found");
        _unitOfWork.Cards.Delete(card);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<CardDto?> EditCard(Guid id, CreateCardDto cardDto)
    {
        var existingCard = await _unitOfWork.Cards.GetByIdAsync(id)
            ?? throw new ObjectNotFoundException("Card not found");

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
