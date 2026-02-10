import type { DeckDto } from "./deck.types";
import type { SerachUserDto } from "./users.types";

export interface GameRoomDto
{
    id:string,
    isRanked:string,
    status:number,
    joinCode?:string,
    hostUserId:SerachUserDto,
    createdAt:Date
}

export interface CreateGameRoomDto
{
    isRanked:string,
    hostUserId:string,
}

export interface EditGameRoomDto
{
  isRanked: boolean,
  status: number,
  joinCode: string,
  createdAt: Date
}

export interface GameRoomUserDto
{
    id:string,
    user:SerachUserDto,
    deck:DeckDto,
    isReady:boolean
}
