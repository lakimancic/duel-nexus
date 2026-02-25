import type { CardDto } from "@/shared/types/card.types";
import type { ShortUserDto } from "@/shared/types/user.types";

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
  id: string;
  playerId: string;
  zone: number;
  deckOrder: number | null;
  isFaceDown: boolean;
  fieldIndex: number | null;
  defensePosition: boolean;
  card: CardDto | null;
}

export interface PlayerGameDto {
  id: string;
  index: number;
  lifePoints: number;
  turnEnded: boolean;
  user: ShortUserDto;
}

export interface TurnDto {
  id: string;
  gameId: string;
  turnNumber: number;
  activePlayerId: string | null;
  phase: TurnPhase | number;
  startedAt: string;
  endedAt: string | null;
}

export interface GameStateDto {
  gameId: string;
  viewerPlayerId: string;
  viewerDrawsInTurn: number;
  currentTurn: TurnDto;
  players: PlayerGameDto[];
  cards: Array<{
    id: string;
    zone: number;
    deckOrder: number | null;
    isFaceDown: boolean;
    fieldIndex: number | null;
    defensePosition: boolean;
    card: CardDto | null;
    playerGameId: string;
  }>;
}
