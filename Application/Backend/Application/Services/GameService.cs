using AutoMapper;
using Backend.Application.DTOs.Decks;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Domain;
using Backend.Utils.Data;

namespace Backend.Application.Services;

public class GameService(IUnitOfWork unitOfWork, IMapper mapper) : IGameService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<CardDto> DrawCardAsync(Guid gameId, Guid playerId)
    {
        var turn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
            ?? throw new InvalidOperationException($"No active turn found for game with ID '{gameId}'");
        if (turn.Phase != TurnPhase.Draw)
            throw new InvalidOperationException($"Cannot draw card during phase '{turn.Phase}'");
        var player = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(playerId, gameId)
            ?? throw new KeyNotFoundException($"Player with ID '{playerId}' not found in game with ID '{gameId}'");
        var gameCards = await _unitOfWork.GameCards.GetByPlayerGameIdAsync(player.Id);
        if (gameCards.Count(gc => gc.Zone == CardZone.Hand) >= GameConstants.MaxHandSize)
            throw new InvalidOperationException("Player's hand is full");
        var deckCard = gameCards.Where(gc => gc.Zone == CardZone.Deck).OrderBy(gc => gc.DeckOrder).FirstOrDefault()
            ?? throw new InvalidOperationException("No cards left in deck to draw");

        deckCard.Zone = CardZone.Hand;
        _unitOfWork.GameCards.Update(deckCard);
        await _unitOfWork.CardMovements.DrawActionAsync(deckCard, turn);
        var card = await _unitOfWork.Cards.GetByIdAsync(deckCard.CardId)
            ?? throw new KeyNotFoundException($"Card with ID '{deckCard.CardId}' not found");
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<Card, CardDto>(card);
    }

    public async Task EndGamePhaseAsync(Guid gameId)
    {
        var turn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
            ?? throw new InvalidOperationException($"No active turn found for game with ID '{gameId}'");
        if (!turn.Phase.HasNext())
            throw new InvalidOperationException($"Cannot advance phase from '{turn.Phase}'");
        turn.Phase = turn.Phase.Next();
        _unitOfWork.Turns.Update(turn);
        await _unitOfWork.CompleteAsync();
    }

    public async Task EndGameTurnAsync(Guid gameId)
    {
        var turn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
            ?? throw new InvalidOperationException($"No active turn found for game with ID '{gameId}'");
        if (turn.Phase.HasNext())
            throw new InvalidOperationException($"Cannot end turn during phase '{turn.Phase}'");
        await _unitOfWork.Turns.NextTurnAsync(turn);
        await _unitOfWork.CompleteAsync();
    }
}
