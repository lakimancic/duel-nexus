namespace Backend.Application.DTOs.Games;

public class GameStateDto
{
    public Guid GameId { get; set; }
    public Guid ViewerPlayerId { get; set; }
    public TurnDto CurrentTurn { get; set; } = null!;
    public List<PlayerGameDto> Players { get; set; } = [];
    public List<GameCardDto> Cards { get; set; } = [];
}
