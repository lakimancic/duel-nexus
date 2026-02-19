import type { CardDto } from "@/shared/types/card.types";

export interface GameCardDto {
  playerId: string;
  zone: number;
  isFaceDown: boolean;
  fieldIndex: number | null;
  defensePosition: boolean;
  card: CardDto|null;
};
