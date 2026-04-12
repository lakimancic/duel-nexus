import { httpClient } from "@/shared/api/httpClient";
import type { ScoreboardEntryDto } from "../types/scoreboard.types";

export const scoreboardApi = {
  getScoreboard: (limit = 50) =>
    httpClient.get<ScoreboardEntryDto[]>(`/users/scoreboard?limit=${limit}`),
};
