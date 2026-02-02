namespace Backend.Application.DTOs.Chat;

public class SendMessageDto
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
}
