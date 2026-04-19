export interface CardEffectDto {
  id: string;
  type: number;
  affects: number | null;
  points: number | null;
  turns: number | null;
  requiresTarget: boolean;
  targetsPlayer: boolean;
}

export interface CardDto {
  id: string;
  name: string;
  image: string;
  description: string;
  type: number;
  effectId: string | null;
  effect?: CardEffectDto | null;
  attack: number | null;
  defense: number | null;
  level: number | null;
}
