import type { ShortUserDto } from "./user.types";

export interface MessageDto {
  content: string;
  senderId: string;
  receiverId: string | null;
  gameRoomId: string | null;
  sender: ShortUserDto;
  sentAt: string;
}

export interface PrivateConversationDto {
  user: ShortUserDto;
  lastMessageContent: string | null;
  lastMessageSentAt: string | null;
}
