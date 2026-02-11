import type { EffectDto } from "./effect.types";

export interface CardDto {
  id: string;
  name: string;
  image: string;
  description: string;
  type: number;

  effectId: number;
  effect?: EffectDto;

  attack?: number;
  defense?: number;
  level?: number;
}

export interface CreateCardDto {
  name: string;
  image: string;
  description: string;
  type: number;

  attack?: number;
  defense?: number;
  level?: number;
}

export interface PlayerCardDto {
  id: string;
  card: CardDto;
  quantity: number;
}

export interface CreatePlayerCardDto {
  cardId: string;
  quantity: number;
}
