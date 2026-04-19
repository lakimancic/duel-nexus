using System.Collections.Concurrent;
using Backend.Domain;

namespace Backend.Application.Services;

public sealed class TrapResponseCoordinator
{
    public sealed record TrapSelection(
        Guid TrapCardId,
        IReadOnlyCollection<Guid>? TargetCardIds,
        IReadOnlyCollection<Guid>? TargetPlayerIds);

    private sealed class PendingTrapWindow
    {
        public required Guid WindowId { get; init; }
        public required Guid GameId { get; init; }
        public required Guid DefenderUserId { get; init; }
        public required Guid DefenderPlayerGameId { get; init; }
        public required DateTimeOffset ExpiresAtUtc { get; init; }
        public required HashSet<Guid> AllowedTrapCardIds { get; init; }
        public required TaskCompletionSource<TrapSelection?> SelectionSource { get; init; }
    }

    private readonly ConcurrentDictionary<Guid, PendingTrapWindow> _windowsById = [];
    private readonly ConcurrentDictionary<Guid, Guid> _windowIdByGameId = [];

    public bool TryOpenWindow(
        Guid gameId,
        Guid defenderUserId,
        Guid defenderPlayerGameId,
        IEnumerable<Guid> allowedTrapCardIds,
        out Guid windowId,
        out DateTimeOffset expiresAtUtc)
    {
        windowId = Guid.Empty;
        expiresAtUtc = DateTimeOffset.UtcNow;

        if (_windowIdByGameId.ContainsKey(gameId))
            return false;

        var trapCardIds = allowedTrapCardIds.ToHashSet();
        if (trapCardIds.Count == 0)
            return false;

        var pendingWindowId = Guid.NewGuid();
        var pendingWindow = new PendingTrapWindow
        {
            WindowId = pendingWindowId,
            GameId = gameId,
            DefenderUserId = defenderUserId,
            DefenderPlayerGameId = defenderPlayerGameId,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(GameConstants.TrapResponseWindowSeconds),
            AllowedTrapCardIds = trapCardIds,
            SelectionSource = new TaskCompletionSource<TrapSelection?>(TaskCreationOptions.RunContinuationsAsynchronously),
        };

        if (!_windowIdByGameId.TryAdd(gameId, pendingWindowId))
            return false;

        if (!_windowsById.TryAdd(pendingWindowId, pendingWindow))
        {
            _windowIdByGameId.TryRemove(gameId, out _);
            return false;
        }

        windowId = pendingWindowId;
        expiresAtUtc = pendingWindow.ExpiresAtUtc;
        return true;
    }

    public bool TryActivateTrap(
        Guid windowId,
        Guid defenderUserId,
        Guid trapCardId,
        IReadOnlyCollection<Guid>? targetCardIds,
        IReadOnlyCollection<Guid>? targetPlayerIds)
    {
        if (!_windowsById.TryGetValue(windowId, out var window))
            return false;

        if (window.DefenderUserId != defenderUserId)
            return false;

        if (DateTimeOffset.UtcNow > window.ExpiresAtUtc)
            return false;

        if (!window.AllowedTrapCardIds.Contains(trapCardId))
            return false;

        return window.SelectionSource.TrySetResult(new TrapSelection(
            TrapCardId: trapCardId,
            TargetCardIds: targetCardIds,
            TargetPlayerIds: targetPlayerIds));
    }

    public async Task<TrapSelection?> WaitForSelectionOrTimeoutAsync(Guid windowId, CancellationToken cancellationToken = default)
    {
        if (!_windowsById.TryGetValue(windowId, out var window))
            return null;

        var timeout = window.ExpiresAtUtc - DateTimeOffset.UtcNow;
        if (timeout <= TimeSpan.Zero)
        {
            CleanupWindow(window);
            return null;
        }

        var delayTask = Task.Delay(timeout, cancellationToken);
        var completed = await Task.WhenAny(window.SelectionSource.Task, delayTask);
        var selection = completed == window.SelectionSource.Task
            ? await window.SelectionSource.Task
            : null;

        CleanupWindow(window);
        return selection;
    }

    public bool IsWindowActive(Guid windowId)
    {
        return _windowsById.ContainsKey(windowId);
    }

    private void CleanupWindow(PendingTrapWindow window)
    {
        _windowsById.TryRemove(window.WindowId, out _);
        _windowIdByGameId.TryRemove(window.GameId, out _);
    }
}
