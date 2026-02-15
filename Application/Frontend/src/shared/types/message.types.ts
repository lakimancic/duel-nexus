import type { ShortUserDto } from "./user.types";

export interface MessageDto {
  content: string;
  sender: ShortUserDto;
  sentAt: string;
}
