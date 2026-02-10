export interface UserDto
{
    id:string,
    username:string,
    email:string,
    role:number,
    Elo:number
}

export interface SerachUserDto
{
    id:string,
    username:string
}

export interface CreateUserDto
{
    username:string,
    email:string,
    password:string
}

export interface UpdateUserDto
{
    username:string,
    email:string,
    role:number,
    elo:number
}