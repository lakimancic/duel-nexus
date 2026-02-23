namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public sealed record RevealCardActionCommand(Guid GameCardId) : IGameCommand<GameCardUpdateResult>;
