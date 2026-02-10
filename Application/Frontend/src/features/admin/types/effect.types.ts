export interface EffectDto
{
    id:string,
    Type:number,
    Affects:number,
    Points:number,
    Turns:number,
    RequiresTarget:boolean
    TargetsPlayer:boolean
}

export interface CreateEffectDto
{
    Type:number,
    Affects:number,
    Points:number,
    Turns:number,
    RequiresTarget:boolean
    TargetsPlayer:boolean
}