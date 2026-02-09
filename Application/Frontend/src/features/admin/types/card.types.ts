export interface CardDto {
  id: string;
  name: string;
  image: string;
  description: string;
  type: number;

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
