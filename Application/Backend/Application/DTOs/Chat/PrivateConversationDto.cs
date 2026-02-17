using Backend.Application.DTOs.Users;

namespace Backend.Application.DTOs.Chat;

public class PrivateConversationDto
{
    public ShortUserDto User { get; set; } = null!;
    public string? LastMessageContent { get; set; }
    public DateTime? LastMessageSentAt { get; set; }
}
