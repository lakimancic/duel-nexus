using System.Collections.Concurrent;
using Backend.Application.DTOs.Games;
using Backend.Application.Services.Interfaces;

namespace Backend.Application.Services;

public class GameResultStore : IGameResultStore
{
    private readonly ConcurrentDictionary<Guid, GameResultDto> _results = new();

    public void Set(GameResultDto result)
    {
        _results[result.GameId] = result;
    }

    public GameResultDto? Get(Guid gameId)
    {
        _results.TryGetValue(gameId, out var result);
        return result;
    }
}
