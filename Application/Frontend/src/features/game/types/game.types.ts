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
  isFinished: boolean;
  result?: GameResultDto | null;
  viewerPlayerId: string;
  viewerDrawsInTurn: number;
  attackedCardIdsInCurrentTurn: string[];
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

export interface BattleAttackResultDto {
  gameId: string;
  roomId: string;
  turnId: string;
  playerGameId: string;
  attackerCardId: string;
  defenderCardId: string | null;
  defenderPlayerGameId: string | null;
  damageToDefender: number;
  damageToAttacker: number;
  attackerDestroyed: boolean;
  defenderDestroyed: boolean;
  attackFailed: boolean;
  phaseAdvanced: boolean;
  turnChanged: boolean;
  activePlayerId: string | null;
  currentPhase: TurnPhase | number;
  activatedEffect: GameEffectActivationDto | null;
}

export interface GameEffectActivationDto {
  sourceCardId: string;
  activatedByPlayerGameId: string;
  sourceCardName: string;
  effectType: number;
  isTrap: boolean;
}

export interface TrapCardOptionDto {
  gameCardId: string;
  cardName: string;
  effectId: string | null;
  effectType: number | null;
  requiresTarget: boolean;
  targetsPlayer: boolean;
  affects: number | null;
}

export interface TrapResponseWindowEventDto {
  gameId: string;
  windowId: string;
  defenderPlayerGameId: string;
  timeoutSeconds: number;
  expiresAtUtc: string;
  availableTrapCards: TrapCardOptionDto[];
}

export interface TrapResponseWindowOpenedEventDto {
  gameId: string;
  windowId: string;
  defenderPlayerGameId: string;
  timeoutSeconds: number;
  expiresAtUtc: string;
}

export interface GameEndPlayerRatingChangeDto {
  playerGameId: string;
  userId: string;
  username: string;
  oldElo: number;
  newElo: number;
  delta: number;
  isWinner: boolean;
}

export interface GameResultDto {
  gameId: string;
  roomId: string;
  isRanked: boolean;
  finishedAt: string;
  winnerPlayerGameId: string | null;
  winnerUserId: string | null;
  winnerUsername: string | null;
  playerRatingChanges: GameEndPlayerRatingChangeDto[];
}
