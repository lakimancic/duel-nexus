namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public interface IGameCommandValidator<in TCommand, TResult>
    where TCommand : IGameCommand<TResult>
{
    Task ValidateAsync(TCommand command, GameCommandContext context, CancellationToken cancellationToken = default);
}
