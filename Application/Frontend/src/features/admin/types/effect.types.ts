export interface EffectDto {
  id: string;
  type: number;
  affects?: number;
  points?: number;
  turns?: number;
  requiresTarget: boolean;
  targetsPlayer: boolean;
}

export interface CreateEffectDto {
  type: number;
  affects?: number;
  points?: number;
  turns?: number;
  requiresTarget: boolean;
  targetsPlayer: boolean;
}

export interface EffectsQuery {
  page: number;
  pageSize: number;
}
