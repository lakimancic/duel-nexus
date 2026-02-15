import { httpClient } from "@/shared/api/httpClient";
import type { ShortUserDto } from "@/shared/types/user.types";

export const connectionApi = {
  getOnlineUsers: () =>
    httpClient.get<ShortUserDto[]>("/connections"),
};
