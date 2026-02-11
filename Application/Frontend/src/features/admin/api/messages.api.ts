import { httpClient } from "@/shared/api/httpClient";
import type { PagedResult, Query } from "@/shared/types/result.types";
import type {
  SendMessageDto,
  MessageDto,
  SendGameRoomMessageDto,
  SendPrivateMessageDto,
  EditMessageDto,
  EditedMessageDto,
} from "../types/message.types";

export const messagesApi = {
  getMessages: (params?: Query) => httpClient.get<PagedResult<MessageDto>>("/admin/messages", {params}),

  getMessageByGameRoomId: (id: number, params?: Query) =>
    httpClient.get<PagedResult<MessageDto>>(`/admin/messages/game-room/${id}`, {params}),

  getPrivateMessage: (userId1: string, userId2: string, params?: Query) =>
    httpClient.get<PagedResult<MessageDto>>(
      `/admin/message/private/${userId1}/${userId2}`, {params}
    ),

  sendMessage: (dto: SendMessageDto) => httpClient.post("/admin/messages", dto),

  sendGameRoomMessage: (dto: SendGameRoomMessageDto) =>
    httpClient.post("/admin/messages/game-room", dto),

  sendPrivateMessage: (dto: SendPrivateMessageDto) =>
    httpClient.post("/admin/messages/private", dto),

  editMessage: (id: string, dto: EditMessageDto) =>
    httpClient.put<EditedMessageDto>(`/admin/messages/${id}`, dto),

  deleteMessage: (id: string) => httpClient.delete(`/admin/messages/${id}`),
};
