import { httpClient } from "@/shared/api/httpClient";
import type { CreateGameAction, CreateGameCardMovementsDto, CreateGamePlaceAction, EditGameDto, EditPlayerGame, EditTurnInGame, GameActionDto, GameCardDto, GameCardMovementsDto, GameDto, GamePlaceActionDto, OnePlayerGames, PlayerGames, TurnInGame } from "../types/game.types";
import type { PagedResult } from "@/shared/types/result.types";
import type { EnumDto } from "@/shared/types/enum.types";

export const gameApi = {

    getGames:() => httpClient.get<PagedResult<GameDto>>('/admin/games'),

    getGamesById:(id:string) => httpClient.get(`/admin/games/${id}`),

    startGame:(roomId:string) =>httpClient.post<GameDto>(`/admin/games`,{roomId}),

    editGame:(id:string, dto:EditGameDto) => httpClient.put<GameDto>(`/admin/games/${id}`,dto),

    
    
    getPlayersGame:(id:string) => httpClient.get<PlayerGames[]>(`/admin/games/${id}/players`),

    getPlayerGameById:(id:string, userId:string) => httpClient.get<OnePlayerGames>(`/admin/games/${id}/players/${userId}`),

    editPlayerGame:(id:string, userId:string, dto:EditPlayerGame) => httpClient.put<OnePlayerGames>(`/admin/games/${id}/players/${userId}`,dto),



    getGamesTurns:(id:string) => httpClient.get<PagedResult<TurnInGame>>(`/admin/games/${id}/turns`),
    
    getGamesTurnById:(roomId:string, turnId:string) => httpClient.get<TurnInGame>(`/admin/games/${roomId}/turns/${turnId}`),

    createGameTurn:(id:string) => httpClient.post<TurnInGame>(`/admin/games/${id}/turns`),

    editGameTurn:(turnId:string, dto:EditTurnInGame) => httpClient.put(`/admin/games/turns/${turnId}`,dto),



    getGameCards:(id:string) => httpClient.get<GameCardDto[]>(`/admin/games/${id}/cards`),
    
    getGameCardById:(cardId:string) => httpClient.get<GameCardDto>(`/admin/games/cards/${cardId}`),

    editGameCard:(cardId:string) => httpClient.delete<GameCardDto>(`/admin/games/cards/${cardId}`),



    getGameActions:(id:string) => httpClient.get<PagedResult<GameActionDto>>(`/admin/games/${id}/attack-actions`),

    getGameActionById:(actionId:string) => httpClient.get<GameActionDto>(`/admin/games/attack-actions/${actionId}`),

    createGameAction:(dto:CreateGameAction) => httpClient.post<GameActionDto>('/admin/games/attack-actions',dto), 

    editGameAction:(id:string,dto:CreateGameAction) => httpClient.put<GameActionDto>(`/admin/games/attack-actions/${id}`,dto), 
    
    deleteGameAction:(id:string) => httpClient.delete(`/admin/games/attack-actions/${id}`), 



    getGameCardMovements:(id:string) => httpClient.get<PagedResult<GameCardMovementsDto>>(`/admin/games/${id}/card-movements`),

    getGameCardMovementById:(id:string) => httpClient.get<GameCardMovementsDto>(`/admin/games/card-movements/${id}`),

    createGameCardMovements:(dto:CreateGameCardMovementsDto) => httpClient.post<GameCardMovementsDto>(`/admin/games/card-movements`,dto),

    editGameCardMovements:(id:string,dto:CreateGameCardMovementsDto) => httpClient.put<GameCardMovementsDto>(`/admin/games/card-movements/${id}`,dto),

    deleteGameCardMovement:(id:string) => httpClient.delete(`/admin/games/card-movement/${id}`),



    // getGameEffectActivations:(id:string) => httpClient.get<PagedResult<>>(`/admin/games/${id}/effect-activations`),

    // getGameEffectActivationById:(id:string) => httpClient.get<>(`/admin/games/effect-activations/${id}`),

    // createGameEffectActivation:(dto:CreateGameEffectActivationDto) => httpClient.post<>(`/admin/games/effect-activations`,dto),

    // editGameEffectActivation:(id:string,dto:CreateGameEffectActivationDto) => httpClient.put<>(`/admin/games/effect-activations/${id}`,dto),

    // deleteGameEffectActivations:(id:string) => httpClient.delete(`/admin/games/effect-activations/${id}`)




    getGamePlaceActions:(id:string) => httpClient.get<PagedResult<GamePlaceActionDto>>(`/admin/games/${id}/place-actions`),

    getGamePlaceActionById:(actionId:string) => httpClient.get<GamePlaceActionDto>(`/admin/games/place-actions/${actionId}`),

    createGamePlaceAction:(dto:CreateGamePlaceAction) => httpClient.post<GamePlaceActionDto>('/admin/games/place-actions',dto), 

    editGamePlaceAction:(id:string,dto:CreateGamePlaceAction) => httpClient.put<GamePlaceActionDto>(`/admin/games/place-actions/${id}`,dto), 
    
    deleteGamePlaceAction:(id:string) => httpClient.delete(`/admin/games/place-actions/${id}`), 



    getMovementType:() => httpClient.get<EnumDto>('/admin/games/movement-types'),
    getZoneype:() => httpClient.get<EnumDto>('/admin/games/card-zones'),
    getPlaceType:() => httpClient.get<EnumDto>('/admin/games/place-types'),
    getTurnPhaseType:() => httpClient.get<EnumDto>('/admin/games/turn-phase')
}