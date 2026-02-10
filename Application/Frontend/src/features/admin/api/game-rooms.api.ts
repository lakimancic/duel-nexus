import { httpClient } from "@/shared/api/httpClient";
import type { InformativeResult, PagedResult } from "@/shared/types/result.types";
import type { CreateGameRoomDto, EditGameRoomDto, GameRoomDto, GameRoomUserDto } from "../types/game-room.types";
import type { EnumDto } from "@/shared/types/enum.types";
import type { SerachUserDto } from "../types/users.types";

export const gameRoomApi = {

    getGameRooms:()=>httpClient.get<PagedResult<GameRoomDto>>('/admin/game-rooms'),

    createGameRoom:(dto:CreateGameRoomDto)=>httpClient.post<CreateGameRoomDto>('/admin/game-rooms',dto),

    getRoomById:(id:string) => httpClient.get<GameRoomDto>(`/admin/game-rooms/${id}`),

    editRoom:(id:string, dto:EditGameRoomDto) => httpClient.put(`/admin/game-room/${id}`,dto),

    getPlayerInGameRoom:(id:string) => httpClient.get<SerachUserDto[]>(`/admin/game-rooms/${id}/players`),

    addPlayerToRoom:(roomId:string,userId:string) => httpClient.post<GameRoomUserDto>(`/admin/game-rooms/${roomId}/players`,{userId}),
    
    editDeckInGameRoom:(roomId:string, userId:string,deckId:string) => httpClient.put(`/admin/game-rooms/${roomId}/players/${userId}`,{deckId}) ,
    
    removePlayerFromRoom: (roomId:string, userId:string) => httpClient.delete<InformativeResult>(`/admin/game-rooms/${roomId}/player/${userId}`),

    getStatuses:()=> httpClient.get<EnumDto>("/admin/game-rooms/statuses")
}