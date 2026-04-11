using Backend.Application.Services.Interfaces;

namespace Backend.Application.Services;

public class RankedMatchmakingWorker(
    IRankedMatchmakingService rankedMatchmaking,
    ILogger<RankedMatchmakingWorker> logger) : BackgroundService
{
    private readonly IRankedMatchmakingService _rankedMatchmaking = rankedMatchmaking;
    private readonly ILogger<RankedMatchmakingWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _rankedMatchmaking.ProcessQueueAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Ranked matchmaking processing failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(4), stoppingToken);
        }
    }
}
