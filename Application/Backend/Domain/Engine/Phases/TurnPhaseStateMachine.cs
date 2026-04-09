namespace Backend.Domain.Engine.Phases;

using Backend.Data.Enums;
using Backend.Utils.WebApi;

public sealed class TurnPhaseStateMachine : ITurnPhaseStateMachine
{
    private readonly IReadOnlyDictionary<TurnPhase, ITurnPhaseState> _states;

    public TurnPhaseStateMachine()
    {
        _states = new Dictionary<TurnPhase, ITurnPhaseState>
        {
            [TurnPhase.Draw] = new DrawPhaseState(),
            [TurnPhase.Main1] = new Main1PhaseState(),
            [TurnPhase.Battle] = new BattlePhaseState(),
            [TurnPhase.Main2] = new Main2PhaseState(),
        };
    }

    public ITurnPhaseState GetState(TurnPhase phase)
    {
        if (!_states.TryGetValue(phase, out var state))
            throw new BadRequestException($"Unsupported phase transition for phase '{phase}'.");

        return state;
    }

    public Task<TurnPhaseTransitionResult> AdvanceAsync(
        TurnPhaseStateContext context,
        TurnPhaseAdvanceTrigger trigger,
        CancellationToken cancellationToken = default)
    {
        var state = GetState(context.Turn.Phase);
        return state.AdvanceAsync(context, trigger, cancellationToken);
    }

    public async Task<TurnPhaseTransitionResult> TryAdvanceOnTimeoutAsync(
        TurnPhaseStateContext context,
        CancellationToken cancellationToken = default)
    {
        if (!_states.TryGetValue(context.Turn.Phase, out var state))
            return TurnPhaseTransitionResult.NoChange(context.Turn);

        if (state.Timeout is null)
            return TurnPhaseTransitionResult.NoChange(context.Turn);

        if (DateTime.UtcNow - context.Turn.StartedAt < state.Timeout.Value)
            return TurnPhaseTransitionResult.NoChange(context.Turn);

        return await state.AdvanceAsync(context, TurnPhaseAdvanceTrigger.Timeout, cancellationToken);
    }
}
