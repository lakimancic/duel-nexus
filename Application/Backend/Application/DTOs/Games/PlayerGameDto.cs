using Backend.Application.DTOs.Users;

namespace Backend.Application.DTOs.Games;

public class PlayerGameDto
{
    public Guid Id { get; set; }
    public int Index { get; set; }
    public int LifePoints { get; set; }
    public bool TurnEnded { get; set; }
    public ShortUserDto User { get; set; } = null!;
}
