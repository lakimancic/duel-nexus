using Backend.Application.DTOs.Users;

namespace Backend.Application.DTOs.Chat;

public class ChatMessageDto
{
    public string Content { get; set; } = null!;
    public Guid SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public Guid? GameRoomId { get; set; }
    public ShortUserDto Sender { get; set; } = null!;
    public DateTime SentAt { get; set; }
}
