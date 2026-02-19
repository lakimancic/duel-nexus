import type { CardDto } from "@/shared/types/card.types";
import type { ShortUserDto } from "@/shared/types/user.types";

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
