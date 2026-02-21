namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public interface IGameCommand<TResult>
{
    Task<TResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default);
}
