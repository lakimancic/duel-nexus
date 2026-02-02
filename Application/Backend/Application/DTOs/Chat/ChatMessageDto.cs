using Backend.Application.DTOs.Users;

namespace Backend.Application.DTOs.Chat;

public class ChatMessageDto
{
    public string Content { get; set; } = null!;
    public ShortUserDto Sender { get; set; } = null!;
    public DateTime SentAt { get; set; }
}
