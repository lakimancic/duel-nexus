using System.Collections.Concurrent;

namespace Backend.Application.Services;

public class ConnectionTracker
{
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _onlineUsers = [];
    private readonly ConcurrentDictionary<string, Guid> _connectionToUser = [];

    public Guid? AddOnlineUser(Guid userId, string connectionId)
    {
        var connections = _onlineUsers.GetOrAdd(userId, _ => []);

        lock (connections)
        {
            var isFirstConnection = connections.Count == 0;
            connections.Add(connectionId);
            _connectionToUser[connectionId] = userId;
            return isFirstConnection ? userId : null;
        }
    }

    public Guid? RemoveOnlineUser(Guid userId, string connectionId)
    {
        _connectionToUser.TryRemove(connectionId, out _);

        if (!_onlineUsers.TryGetValue(userId, out var connections))
            return null;

        lock (connections)
        {
            connections.Remove(connectionId);

            if (connections.Count > 0)
                return null;

            _onlineUsers.TryRemove(userId, out _);
            return userId;
        }
    }

    public List<string> GetConnectionByUser(Guid userId)
    {
        if (!_onlineUsers.TryGetValue(userId, out var connections))
            return [];

        lock (connections)
        {
            return [.. connections];
        }
    }

    public List<Guid> GetOnlineUserIds()
    {
        return [.. _onlineUsers.Keys];
    }
}
