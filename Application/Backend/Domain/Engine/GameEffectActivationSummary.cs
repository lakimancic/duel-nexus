namespace Backend.Domain.Engine;

using Backend.Data.Enums;

public sealed record GameEffectActivationSummary(
    Guid SourceCardId,
    Guid ActivatedByPlayerGameId,
    string SourceCardName,
    EffectType EffectType,
    bool IsTrap
);
