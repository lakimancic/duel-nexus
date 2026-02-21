namespace Backend.Application.DTOs.Games;

public class DrawActionResultDto
{
    public Guid GameId { get; set; }
    public Guid RoomId { get; set; }
    public Guid PlayerGameId { get; set; }
    public Guid TurnId { get; set; }
    public int DrawsInTurn { get; set; }
    public bool TurnEnded { get; set; }
    public Guid? NextActivePlayerId { get; set; }
    public GameCardDto DrawnCard { get; set; } = null!;
}
