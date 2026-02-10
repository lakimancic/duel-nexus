import type { SerachUserDto } from "./users.types"

export interface DeckDto {
    id:string,
    user?:SerachUserDto,
    name:string,
    isCompleted:true
}

export interface CreateDeckDto
{
    userId:number,
    name:number
}

export interface AddCardInDeckDto
{
    userId:number,
    quantity:number
}

export interface EditDeckDto
{
    name:string,
    isComplete:boolean
}