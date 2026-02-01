using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class ChatMessage
{
    [Key]
    public Guid Id { get; set; }

    public MessageType Type { get; set; }

    public Guid SenderId { get; set; }
    [ForeignKey(nameof(SenderId))]
    public User Sender { get; set; } = null!;

    public Guid? ReceiverId { get; set; }
    [ForeignKey(nameof(ReceiverId))]
    public User? Receiver { get; set; }

    public Guid? GameRoomId { get; set; }
    [ForeignKey(nameof(GameRoomId))]
    public GameRoom? GameRoom { get; set; }

    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
