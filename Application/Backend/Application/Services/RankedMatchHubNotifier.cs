using Backend.Application.DTOs.GameRooms;
using Backend.Application.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Application.Services;

public class RankedMatchHubNotifier(IHubContext<GameHub> hubContext) : IRankedMatchObserver
{
    private readonly IHubContext<GameHub> _hubContext = hubContext;

    public Task OnPlayerQueuedAsync(RankedQueueJoinedEventDto entry, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.User(entry.UserId.ToString())
            .SendAsync("ranked:queue:joined", entry, cancellationToken);
    }

    public Task OnPlayerDequeuedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.User(userId.ToString())
            .SendAsync("ranked:queue:left", cancellationToken);
    }

    public Task OnMatchFoundAsync(Guid userId, RankedMatchFoundEventDto match, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.User(userId.ToString())
            .SendAsync("ranked:match:found", match, cancellationToken);
    }
}
