import { httpClient } from "@/shared/api/httpClient";
import type { MessageDto, PrivateConversationDto } from "@/shared/types/message.types";
import type { PagedResult, Query } from "@/shared/types/result.types";

export const chatApi = {
  getMessages: (params?: Query) =>
    httpClient.get<PagedResult<MessageDto>>("/chat", { params }),
  getPrivateMessages: (userId: string, params?: Query) =>
    httpClient.get<PagedResult<MessageDto>>(`/chat/private/${userId}`, { params }),
  getPrivateConversations: () =>
    httpClient.get<PrivateConversationDto[]>("/chat/private/conversations"),
};
