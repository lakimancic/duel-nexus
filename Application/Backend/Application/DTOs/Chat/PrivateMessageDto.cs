namespace Backend.Application.DTOs.Chat;

public class PrivateMessageDto
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
    public Guid ReceiverId { get; set; }
}
