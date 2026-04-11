using Backend.Application.DTOs.GameRooms;

namespace Backend.Application.Services.Interfaces;

public interface IRankedMatchObserver
{
    Task OnPlayerQueuedAsync(RankedQueueJoinedEventDto entry, CancellationToken cancellationToken = default);
    Task OnPlayerDequeuedAsync(Guid userId, CancellationToken cancellationToken = default);
    Task OnMatchFoundAsync(Guid userId, RankedMatchFoundEventDto match, CancellationToken cancellationToken = default);
}
