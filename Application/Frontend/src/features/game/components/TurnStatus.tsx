import { TurnPhase, type GameTurnStatus } from "@/features/game/types/game.types";

interface TurnStatusProps {
  status: GameTurnStatus;
}

const TURN_PHASES: Array<{ value: TurnPhase; label: string }> = [
  { value: TurnPhase.Draw, label: "Draw" },
  { value: TurnPhase.Main1, label: "Main1" },
  { value: TurnPhase.Battle, label: "Battle" },
  { value: TurnPhase.Main2, label: "Main2" },
  { value: TurnPhase.End, label: "End" },
];

const TurnStatus = ({ status }: TurnStatusProps) => {
  const activePhase = Number(status.phase);
  const activePhaseLabel =
    TURN_PHASES.find((phase) => phase.value === activePhase)?.label ?? `Unknown (${status.phase})`;

  return (
    <section className="inline-flex items-center gap-4 rounded-lg border border-cyan-300/35 bg-black/55 px-3 py-2 backdrop-blur-sm">
      <p className="text-xs text-white/80">
        Player: <span className="font-semibold text-white">{status.activePlayerId}</span>
      </p>
      <p className="text-xs text-white/80">
        Phase: <span className="font-semibold text-cyan-200">{activePhaseLabel}</span>
      </p>
    </section>
  );
};

export default TurnStatus;
