namespace Backend.Application.DTOs.Chat;

public class GameRoomMessageDto
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public Guid GameRoomId { get; set; }
}
