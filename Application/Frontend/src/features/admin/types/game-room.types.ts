import type { DeckDto } from "./deck.types";
import type { SerachUserDto } from "./users.types";

export interface GameRoomDto
{
    id:string,
    isRanked:string,
    status:number,
    joinCode?:string,
    hostUserId:SerachUserDto,
    createdAt:string
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
  createdAt: string
}

export interface GameRoomUserDto
{
    id:string,
    user:SerachUserDto,
    deck?:DeckDto,
    isReady:boolean
}