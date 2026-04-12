import { httpClient } from "@/shared/api/httpClient";
import type { UserProfileStatsDto } from "../types/profile.types";

export const profileApi = {
  getMyProfileStats: (gamesLimit = 20) =>
    httpClient.get<UserProfileStatsDto>(`/users/me/profile-stats?gamesLimit=${gamesLimit}`),
};
