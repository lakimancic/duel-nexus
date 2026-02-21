import type { CardDto } from "@/shared/types/card.types";

export const TurnPhase = {
  Draw: 0,
  Main1: 1,
  Battle: 2,
  Main2: 3,
  End: 4,
} as const;
export type TurnPhase = (typeof TurnPhase)[keyof typeof TurnPhase];

export interface GameTurnStatus {
  activePlayerId: string;
  phase: TurnPhase | number;
}

export interface GameCardDto {
  playerId: string;
  zone: number;
  isFaceDown: boolean;
  fieldIndex: number | null;
  defensePosition: boolean;
  card: CardDto|null;
};
