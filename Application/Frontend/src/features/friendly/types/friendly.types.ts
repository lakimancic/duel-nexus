import type { MessageDto } from "@/shared/types/message.types";
import type { ShortUserDto } from "@/shared/types/user.types";

export interface DeckDto {
  id: string;
  user: ShortUserDto;
  name: string;
  isComplete: boolean;
}

export interface GameRoomDto {
  id: string;
  isRanked: boolean;
  status: number;
  joinCode?: string | null;
  hostUserId: string;
  createdAt: string;
  hostUser: ShortUserDto;
}

export interface GameRoomPlayerDto {
  id: string;
  user: ShortUserDto;
  deck?: DeckDto | null;
  isReady: boolean;
}

export interface StartRoomResponse {
  gameId: string;
}

export interface GameStartedEventDto {
  roomId: string;
  gameId: string;
}

export interface LeaveRoomResponse {
  cancelled: boolean;
}

export interface FriendlyNavigationState {
  message?: string;
}

export interface GameRoomChatState {
  messages: MessageDto[];
  isLoading: boolean;
  isLoadingMore: boolean;
  isSending: boolean;
  canSeeMore: boolean;
  onSeeMore: () => Promise<void>;
  onSend: (content: string) => Promise<void>;
}
