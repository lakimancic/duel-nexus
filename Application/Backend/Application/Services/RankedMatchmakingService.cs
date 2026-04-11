using System.Collections.Concurrent;
using Backend.Application.DTOs.GameRooms;
using Backend.Application.DTOs.Games;
using Backend.Application.Services.Interfaces;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Utils.WebApi;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application.Services;

public class RankedMatchmakingService(
    IServiceScopeFactory scopeFactory,
    IEnumerable<IRankedMatchObserver> observers) : IRankedMatchmakingService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IReadOnlyList<IRankedMatchObserver> _observers = [.. observers];
    private readonly ConcurrentDictionary<Guid, RankedQueueEntry> _queue = new();
    private readonly SemaphoreSlim _matchmakingGate = new(1, 1);

    public async Task<RankedQueueJoinedEventDto> JoinQueue(Guid userId, CancellationToken cancellationToken = default)
    {
        if (_queue.TryGetValue(userId, out var existing))
        {
            var existingEvent = BuildQueueJoinedEvent(existing);
            await NotifyQueuedAsync(existingEvent, cancellationToken);
            return existingEvent;
        }

        var entry = await CreateQueueEntryAsync(userId, cancellationToken);
        _queue[userId] = entry;

        var queueEvent = BuildQueueJoinedEvent(entry);
        await NotifyQueuedAsync(queueEvent, cancellationToken);
        await ProcessQueueAsync(cancellationToken);
        return queueEvent;
    }

    public async Task<bool> LeaveQueue(Guid userId, CancellationToken cancellationToken = default)
    {
        var removed = _queue.TryRemove(userId, out _);
        if (!removed)
            return false;

        await NotifyDequeuedAsync(userId, cancellationToken);
        return true;
    }

    public async Task ProcessQueueAsync(CancellationToken cancellationToken = default)
    {
        await _matchmakingGate.WaitAsync(cancellationToken);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pair = TryPickPair();
                if (pair is null)
                    break;

                if (!_queue.TryRemove(pair.Value.Left.UserId, out _))
                    continue;
                if (!_queue.TryRemove(pair.Value.Right.UserId, out _))
                {
                    _queue[pair.Value.Left.UserId] = pair.Value.Left;
                    continue;
                }

                RankedMatchFoundEventDto? matchResult = null;
                try
                {
                    matchResult = await CreateRankedMatchAsync(pair.Value.Left, pair.Value.Right, cancellationToken);
                }
                catch
                {
                    _queue[pair.Value.Left.UserId] = pair.Value.Left;
                    _queue[pair.Value.Right.UserId] = pair.Value.Right;
                    throw;
                }

                await NotifyMatchFoundAsync(pair.Value.Left.UserId, matchResult, cancellationToken);
                await NotifyMatchFoundAsync(pair.Value.Right.UserId, matchResult, cancellationToken);
            }
        }
        finally
        {
            _matchmakingGate.Release();
        }
    }

    private static RankedQueueJoinedEventDto BuildQueueJoinedEvent(RankedQueueEntry entry)
    {
        return new RankedQueueJoinedEventDto
        {
            UserId = entry.UserId,
            Position = 1,
            QueueSize = 1,
            QueuedAt = entry.QueuedAt,
            Elo = entry.Elo,
        };
    }

    private async Task<RankedQueueEntry> CreateQueueEntryAsync(Guid userId, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var user = await unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new ObjectNotFoundException("User not found.");

        var completeDeck = (await unitOfWork.Decks.GetByUserId(userId))
            .Where(deck => deck.IsComplete)
            .OrderBy(deck => deck.Name)
            .FirstOrDefault();

        if (completeDeck is null)
            throw new BadRequestException("You need at least one complete deck to play ranked matchmaking.");

        return new RankedQueueEntry(
            UserId: user.Id,
            DeckId: completeDeck.Id,
            Elo: user.Elo,
            QueuedAt: DateTime.UtcNow);
    }

    private (RankedQueueEntry Left, RankedQueueEntry Right)? TryPickPair()
    {
        if (_queue.Count < 2)
            return null;

        var now = DateTime.UtcNow;
        var entries = _queue.Values
            .OrderBy(entry => entry.QueuedAt)
            .ToList();

        RankedQueueEntry? bestA = null;
        RankedQueueEntry? bestB = null;
        double bestGap = double.MaxValue;

        for (var i = 0; i < entries.Count - 1; i++)
        {
            for (var j = i + 1; j < entries.Count; j++)
            {
                var left = entries[i];
                var right = entries[j];
                var eloGap = Math.Abs(left.Elo - right.Elo);
                var allowedGap = CalculateAllowedGap(left, right, now);
                if (eloGap > allowedGap)
                    continue;

                if (eloGap >= bestGap)
                    continue;

                bestGap = eloGap;
                bestA = left;
                bestB = right;
            }
        }

        if (bestA is null || bestB is null)
            return null;

        return (bestA.Value, bestB.Value);
    }

    private static double CalculateAllowedGap(RankedQueueEntry left, RankedQueueEntry right, DateTime now)
    {
        var leftWaitMinutes = (now - left.QueuedAt).TotalMinutes;
        var rightWaitMinutes = (now - right.QueuedAt).TotalMinutes;
        var waitMinutes = Math.Max(leftWaitMinutes, rightWaitMinutes);

        return Math.Min(600, 80 + (waitMinutes * 40));
    }

    private async Task<RankedMatchFoundEventDto> CreateRankedMatchAsync(
        RankedQueueEntry left,
        RankedQueueEntry right,
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();

        var room = new GameRoom
        {
            IsRanked = true,
            Status = RoomStatus.Started,
            HostUserId = left.UserId,
            CreatedAt = DateTime.UtcNow,
        };
        await unitOfWork.GameRooms.AddAsync(room);

        var leftPlayer = new GameRoomPlayer
        {
            GameRoom = room,
            UserId = left.UserId,
            DeckId = left.DeckId,
        };
        var rightPlayer = new GameRoomPlayer
        {
            GameRoom = room,
            UserId = right.UserId,
            DeckId = right.DeckId,
        };

        await unitOfWork.GameRoomPlayers.AddAsync(leftPlayer);
        await unitOfWork.GameRoomPlayers.AddAsync(rightPlayer);
        await unitOfWork.CompleteAsync();

        var game = await gameService.CreateGame(new CreateGameDto { RoomId = room.Id });
        await unitOfWork.CompleteAsync();

        return new RankedMatchFoundEventDto
        {
            RoomId = room.Id,
            GameId = game.Id,
        };
    }

    private async Task NotifyQueuedAsync(RankedQueueJoinedEventDto entry, CancellationToken cancellationToken)
    {
        var orderedQueue = _queue.Values
            .OrderBy(queueEntry => queueEntry.QueuedAt)
            .Select(queueEntry => queueEntry.UserId)
            .ToList();
        var position = orderedQueue.FindIndex(id => id == entry.UserId) + 1;

        entry.Position = Math.Max(1, position);
        entry.QueueSize = Math.Max(1, orderedQueue.Count);

        foreach (var observer in _observers)
        {
            await observer.OnPlayerQueuedAsync(entry, cancellationToken);
        }
    }

    private async Task NotifyDequeuedAsync(Guid userId, CancellationToken cancellationToken)
    {
        foreach (var observer in _observers)
        {
            await observer.OnPlayerDequeuedAsync(userId, cancellationToken);
        }
    }

    private async Task NotifyMatchFoundAsync(Guid userId, RankedMatchFoundEventDto match, CancellationToken cancellationToken)
    {
        foreach (var observer in _observers)
        {
            await observer.OnMatchFoundAsync(userId, match, cancellationToken);
        }
    }

    private readonly record struct RankedQueueEntry(
        Guid UserId,
        Guid DeckId,
        double Elo,
        DateTime QueuedAt);
}
