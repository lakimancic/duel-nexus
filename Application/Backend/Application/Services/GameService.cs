using AutoMapper;
using Backend.Application.DTOs.Cards;
using Backend.Application.DTOs.Games;
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

    public async Task ActivateEffectAsync(Guid gameId, Guid playerId, ActivateEffectDto effectDto)
    {
        var turn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
            ?? throw new InvalidOperationException($"No active turn found for game with ID '{gameId}'");
        var player = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(playerId, gameId)
            ?? throw new KeyNotFoundException($"Player with ID '{playerId}' not found in game with ID '{gameId}'");
        var gameCards = await _unitOfWork.GameCards.GetByPlayerGameIdAsync(player.Id);
        var gameCard = gameCards.FirstOrDefault(gc => gc.Id == effectDto.CardId)
            ?? throw new KeyNotFoundException($"Card with ID '{effectDto.CardId}' not found in player's cards");
        if (gameCard.Zone != CardZone.Hand && gameCard.Zone != CardZone.Field)
            throw new InvalidOperationException("Can only activate effect cards from hand or from the field");
        var card = await _unitOfWork.Cards.GetCardWithEffectAsync(effectDto.CardId);
        if (card!.Effect == null)
            throw new InvalidOperationException($"Card with ID '{effectDto.CardId}' doesn't have an effect");

        gameCard.Zone = CardZone.Grave;
        _unitOfWork.GameCards.Update(gameCard);
        var activation = await _unitOfWork.EffectActivations.ActivateEffectAsync(turn, card.Effect, gameCard);
        foreach (var targetDto in effectDto.Targets)
        {
            activation.Targets.Add(new EffectTarget
            {
                Activation = activation,
                TargetCardId = targetDto.CardId,
                TargetPlayerId = targetDto.PlayerId
            });
        }
        await _unitOfWork.CompleteAsync();
    }

    public async Task AttackActionAsync(Guid gameId, Guid playerId, AttackActionDto attack)
    {
        var turn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
            ?? throw new InvalidOperationException($"No active turn found for game with ID '{gameId}'");
        var player = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(playerId, gameId)
            ?? throw new KeyNotFoundException($"Player with ID '{playerId}' not found in game with ID '{gameId}'");
        var gameCards = await _unitOfWork.GameCards.GetByPlayerGameIdAsync(player.Id);
        var gameCard = gameCards.FirstOrDefault(gc => gc.Id == attack.CardId)
            ?? throw new KeyNotFoundException($"Card with ID '{attack.CardId}' not found in player's cards");
        if (gameCard.Zone != CardZone.Field)
            throw new InvalidOperationException("Can only attack with cards from the field");

        await _unitOfWork.Attacks.AddAsync(new AttackAction
        {
            Turn = turn,
            Attacker = gameCard,
            DefenderCardId = attack.TargetCardId,
            DefenderPlayerGameId = attack.TargetPlayerId
        });
        await _unitOfWork.CompleteAsync();
    }

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

    public async Task PlayCardAsync(Guid gameId, Guid playerId, PlayCardDto playCard)
    {
        var turn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
            ?? throw new InvalidOperationException($"No active turn found for game with ID '{gameId}'");
        if (turn.Phase != TurnPhase.Main1 && turn.Phase != TurnPhase.Main2)
            throw new InvalidOperationException($"Cannot draw card during phase '{turn.Phase}'");
        var player = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(playerId, gameId)
            ?? throw new KeyNotFoundException($"Player with ID '{playerId}' not found in game with ID '{gameId}'");
        var gameCards = await _unitOfWork.GameCards.GetByPlayerGameIdAsync(player.Id);
        var gameCard = gameCards.FirstOrDefault(gc => gc.Id == playCard.CardId)
            ?? throw new KeyNotFoundException($"Card with ID '{playCard.CardId}' not found in player's cards");
        if (gameCard.Zone != CardZone.Hand)
            throw new InvalidOperationException("Can only play cards from hand");
        if (gameCards.Any(gc => gc.Zone == CardZone.Field && gc.FieldIndex == playCard.FieldIndex))
            throw new InvalidOperationException($"Field index '{playCard.FieldIndex}' is already occupied");

        await _unitOfWork.PlaceCards.PlayCardActionAsync(
            gameCard, turn, playCard.FieldIndex, playCard.FaceDown, playCard.DefensePosition, PlaceType.NormalSummon
        );
        gameCard.Zone = CardZone.Field;
        _unitOfWork.GameCards.Update(gameCard);
        await _unitOfWork.CompleteAsync();
    }
}
