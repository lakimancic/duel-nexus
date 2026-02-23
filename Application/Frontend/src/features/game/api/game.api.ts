import { httpClient } from "@/shared/api/httpClient";
import type { GameStateDto } from "../types/game.types";

export const gameApi = {
  getGameState: (gameId: string) => httpClient.get<GameStateDto>(`/games/${gameId}/state`),
};
