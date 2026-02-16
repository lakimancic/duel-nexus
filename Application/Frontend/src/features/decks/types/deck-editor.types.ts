import type { ShortUserDto } from "@/shared/types/user.types";

export interface CardDto {
  id: string;
  name: string;
  image: string;
  description: string;
  type: number;
  effectId: string | null;
  attack?: number;
  defense?: number;
  level?: number;
}

export interface PlayerCardDto {
  id: string;
  card: CardDto;
  quantity: number;
}

export interface DeckDto {
  id: string;
  user: ShortUserDto;
  name: string;
  isComplete: boolean;
}

export interface DeckCardDto {
  card: CardDto;
  quantity: number;
}

export interface DeckLimitsDto {
  maxDecks: number;
  maxDeckSize: number;
}
