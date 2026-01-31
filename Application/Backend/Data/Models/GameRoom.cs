using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class GameRoom
{
    [Key]
    public Guid Id { get; set; }

    public bool IsRanked { get; set; }
    public RoomStatus Status { get; set; }

    public string? JoinCode { get; set; }

    public Guid HostUserId { get; set; }
    [ForeignKey(nameof(HostUserId))]
    public User HostUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public ICollection<GameRoomPlayer> Players { get; set; } = [];
}
