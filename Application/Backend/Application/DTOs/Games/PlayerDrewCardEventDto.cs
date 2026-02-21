namespace Backend.Application.DTOs.Games;

public class PlayerDrewCardEventDto
{
    public Guid GameId { get; set; }
    public Guid PlayerGameId { get; set; }
    public Guid TurnId { get; set; }
    public int DrawsInTurn { get; set; }
    public bool TurnEnded { get; set; }
    public Guid? NextActivePlayerId { get; set; }
}
