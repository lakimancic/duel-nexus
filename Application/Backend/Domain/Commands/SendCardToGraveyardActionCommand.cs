namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public sealed record SendCardToGraveyardActionCommand(Guid GameCardId) : IGameCommand<GameCardUpdateResult>;
