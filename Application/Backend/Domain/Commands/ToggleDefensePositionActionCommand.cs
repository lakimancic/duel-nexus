namespace Backend.Domain.Commands;

using Backend.Domain.Engine;
using Backend.Utils.WebApi;

public sealed record ToggleDefensePositionActionCommand(Guid GameCardId) : IGameCommand<GameCardUpdateResult>
{
    public async Task<GameCardUpdateResult> ExecuteAsync(GameCommandContext context, CancellationToken cancellationToken = default)
    {
        var card = await context.UnitOfWork.GameCards.GetByWithCardById(GameCardId)
            ?? throw new ObjectNotFoundException("Game card not found.");

        card.DefensePosition = !card.DefensePosition;
        context.UnitOfWork.GameCards.Update(card);

        return new GameCardUpdateResult(
            Game: context.Game,
            Turn: context.CurrentTurn,
            Player: context.Actor,
            Card: card,
            CurrentPhase: context.CurrentTurn.Phase
        );
    }
}
