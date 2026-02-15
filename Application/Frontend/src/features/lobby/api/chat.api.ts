import { httpClient } from "@/shared/api/httpClient";
import type { MessageDto } from "@/shared/types/message.types";
import type { PagedResult, Query } from "@/shared/types/result.types";

export const chatApi = {
  getMessages: (params?: Query) =>
    httpClient.get<PagedResult<MessageDto>>("/chat", { params }),
};
