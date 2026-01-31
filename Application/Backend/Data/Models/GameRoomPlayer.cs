using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Backend.Data.Models;

public class GameRoomPlayer
{
    [Key]
    public Guid Id { get; set; }

    public Guid GameRoomId { get; set; }
    [ForeignKey(nameof(GameRoomId))]
    public GameRoom GameRoom { get; set; } = null!;

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    public Guid? DeckId { get; set; }
    [ForeignKey(nameof(DeckId))]
    public Deck? Deck { get; set; }

    public bool IsReady { get; set; }
}
