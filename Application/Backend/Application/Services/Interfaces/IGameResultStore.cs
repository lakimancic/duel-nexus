using Backend.Application.DTOs.Games;

namespace Backend.Application.Services.Interfaces;

public interface IGameResultStore
{
    void Set(GameResultDto result);
    GameResultDto? Get(Guid gameId);
}
