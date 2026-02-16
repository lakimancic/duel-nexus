import { httpClient } from "@/shared/api/httpClient";
import type { MessageDto } from "@/shared/types/message.types";
import type { PagedResult, Query } from "@/shared/types/result.types";
import type {
  DeckDto,
  GameRoomDto,
  GameRoomPlayerDto,
  LeaveRoomResponse,
  StartRoomResponse,
} from "../types/friendly.types";

export const friendlyApi = {
  createFriendlyRoom: () => httpClient.post<GameRoomDto>("/game-rooms/friendly"),

  joinFriendlyRoom: (joinCode: string) =>
    httpClient.post<GameRoomDto>("/game-rooms/friendly/join", { joinCode }),

  getRoomById: (roomId: string) => httpClient.get<GameRoomDto>(`/game-rooms/${roomId}`),

  getRoomPlayers: (roomId: string) =>
    httpClient.get<GameRoomPlayerDto[]>(`/game-rooms/${roomId}/players`),

  setMyDeck: (roomId: string, deckId: string) =>
    httpClient.put<GameRoomPlayerDto>(`/game-rooms/${roomId}/deck`, { deckId }),

  leaveRoom: (roomId: string) =>
    httpClient.delete<LeaveRoomResponse>(`/game-rooms/${roomId}/leave`),

  cancelRoom: (roomId: string) => httpClient.post(`/game-rooms/${roomId}/cancel`),

  startRoomGame: (roomId: string) =>
    httpClient.post<StartRoomResponse>(`/game-rooms/${roomId}/start`),

  getMyCompleteDecks: () => httpClient.get<DeckDto[]>("/decks/me/complete"),

  getGameRoomMessages: (roomId: string, params?: Query) =>
    httpClient.get<PagedResult<MessageDto>>(`/chat/game-room/${roomId}`, { params }),
};
