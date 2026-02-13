export interface MessageDto
{
    id:string,
    type:number,
    senderId:string,
    receiverId:string,
    gameRoomId:string,
    content:string,
    sentAt:string
}

export interface SendMessageDto
{
    senderId:string,
    content:string
}

export interface SendGameRoomMessageDto
{
    senderId:string,
    content:string,
    gameRoomId:string
}

export interface SendPrivateMessageDto
{
    senderId:string,
    content:string,
    receiverId:string
}

export interface EditMessageDto
{
    content:string
}

export interface EditedMessageDto
{
    contetn:string,
    sender:string,
    sentAt:string
}