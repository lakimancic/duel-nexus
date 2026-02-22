namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public interface IGameCommandHandler<in TCommand, TResult>
    where TCommand : IGameCommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, GameCommandContext context, CancellationToken cancellationToken = default);
}
