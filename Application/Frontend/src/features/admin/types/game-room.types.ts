import type { DeckDto } from "./deck.types";
import type { SearchUserDto } from "./users.types";

export interface GameRoomDto {
  id: string;
  isRanked: string;
  status: number;
  joinCode?: string;
  hostUserId: SearchUserDto;
  createdAt: string;
}

export interface CreateGameRoomDto {
  isRanked: string;
  hostUserId: string;
}

export interface EditGameRoomDto {
  isRanked: boolean;
  status: number;
  joinCode: string;
  createdAt: string;
}

export interface GameRoomUserDto {
  id: string;
  user: SearchUserDto;
  deck?: DeckDto;
  isReady: boolean;
}
