namespace Backend.Domain.Commands;

using Backend.Domain.Engine;

public sealed record PlaceCardActionCommand(Guid GameCardId, int FieldIndex, bool FaceDown) : IGameCommand<PlaceCardResult>;
