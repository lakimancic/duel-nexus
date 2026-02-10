import type { CardDto } from "./card.types"
import type { SerachUserDto } from "./users.types"

export interface GameDto
{
    id: string,
    startedAt: Date,
    finishedAt?: Date,
    roomId: string
}

export interface EditGameDto
{
    startAt:string,
    finishedAt:string
}

export interface PlayerGames
{
    id:string,
    gameId:string,
    user:SerachUserDto,
    lifePoints:number
}

export interface OnePlayerGames
{
    id:string,
    index:number,
    lifePoints:number,
    user?:SerachUserDto
}

export interface EditPlayerGame
{
    index:number,
    lifePoints:number
}

export interface TurnInGame
{
    id: string,
    gameId: string,
    turnNumber: number,
    activePlayerId: string,
    phase: number,
    startedAt: string,
    endedAt?: string
}

export interface TurnInAttackDto
{
    id:string,
    gameId:string,
    turnNumber:number
}

export interface EditTurnInGame
{
    turnNumber:number,
    activePlayerId:string,
    phase:number,
    statredAt:string,
    endedAt?:string
}

export interface GameCardDto
{
    id:string,
    zone:number,
    deckOrder:number,
    isFaceDown?:boolean,
    defensePosition:boolean,
    card?:CardDto,
    playerGameId:string
}

export interface CreateGameAction
{
    turnId:string,
    attackerCardId:string,
    defenderCardId:string,
    defenderPlayerGameId:string
}

export interface GameActionDto
{
    id:string,
    turn:TurnInAttackDto,
    attacker: GameCardDto,
    defender: GameCardDto,
    defenderPlayer:OnePlayerGames
}

export interface CreateGameCardMovementsDto
{
    turnId:string,
    gameCardId:string,
    fromZone:number,
    toZone:number,
    movementType:number
}

export interface GameCardMovementsDto
{
    id:string,
    turn:TurnInAttackDto,
    gameCard?:GameCardDto,
    fromZone:number,
    toZone:number,
    movementType:number,
    executedAt:string
}

// export interface CreateGameEffectActivationDto
// {
//     turnId:string,
//     effectId:string,
//     sourceCardId:string,
//     resolved:boolean
// }

export interface CreateGamePlaceAction
{
    turnId:string,
    gameCardId:string,
    fieldIndex:number,
    faceDown:boolean,
    defensePosition:boolean,
    type:number
}

export interface GamePlaceActionDto
{
    turn:TurnInAttackDto,
    gameCard?:GameCardDto,
    fieldIndex:number,
    faceDown:boolean,
    defensePosition:boolean,
    type:number
}