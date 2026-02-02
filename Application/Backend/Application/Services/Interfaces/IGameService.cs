using Backend.Application.DTOs.Decks;

namespace Backend.Application.Services.Interfaces;

public interface IGameService
{
    Task<CardDto> DrawCardAsync(Guid gameId, Guid playerId);
    Task EndGamePhaseAsync(Guid gameId);
    Task EndGameTurnAsync(Guid gameId);
}
