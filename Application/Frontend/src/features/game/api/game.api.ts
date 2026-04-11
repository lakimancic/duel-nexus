import { httpClient } from "@/shared/api/httpClient";
import type { GameResultDto, GameStateDto } from "../types/game.types";

export const gameApi = {
  getGameState: (gameId: string) => httpClient.get<GameStateDto>(`/games/${gameId}/state`),
  getGameResult: (gameId: string) => httpClient.get<GameResultDto>(`/games/${gameId}/result`),
};
