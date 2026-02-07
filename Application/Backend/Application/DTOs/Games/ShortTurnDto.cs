namespace Backend.Application.DTOs.Games;

public class ShortTurnDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public int TurnNumber { get; set; }
}
