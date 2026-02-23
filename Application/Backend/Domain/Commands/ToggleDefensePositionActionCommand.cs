namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public sealed record ToggleDefensePositionActionCommand(Guid GameCardId) : IGameCommand<GameCardUpdateResult>;
