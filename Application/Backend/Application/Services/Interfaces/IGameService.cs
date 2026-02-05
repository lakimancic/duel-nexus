using Backend.Application.DTOs.Cards;
using Backend.Application.DTOs.Games;

namespace Backend.Application.Services.Interfaces;

public interface IGameService
{
    Task ActivateEffectAsync(Guid gameId, Guid playerId, ActivateEffectDto effectDto);
    Task<CardDto> DrawCardAsync(Guid gameId, Guid playerId);
    Task EndGamePhaseAsync(Guid gameId);
    Task EndGameTurnAsync(Guid gameId);
    Task PlayCardAsync(Guid gameId, Guid playerId, PlayCardDto playCard);
    Task AttackActionAsync(Guid gameId, Guid playerId, AttackActionDto attack);
}
