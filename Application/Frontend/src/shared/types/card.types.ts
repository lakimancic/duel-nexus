export interface CardDto {
  id: string;
  name: string;
  image: string;
  description: string;
  type: number;
  effectId: string | null;
  attack: number | null;
  defense: number | null;
  level: number | null;
}
