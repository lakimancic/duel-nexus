using Backend.Application.DTOs.GameRooms;

namespace Backend.Application.Services.Interfaces;

public interface IRankedMatchmakingService
{
    Task<RankedQueueJoinedEventDto> JoinQueue(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> LeaveQueue(Guid userId, CancellationToken cancellationToken = default);
    Task ProcessQueueAsync(CancellationToken cancellationToken = default);
}
