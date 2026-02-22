namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public sealed record SkipDrawActionCommand : IGameCommand<DrawPhaseProgressResult>;
