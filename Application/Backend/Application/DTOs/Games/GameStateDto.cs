namespace Backend.Application.DTOs.Games;

public class GameStateDto
{
    public Guid GameId { get; set; }
    public bool IsFinished { get; set; }
    public GameResultDto? Result { get; set; }
    public Guid ViewerPlayerId { get; set; }
    public int ViewerDrawsInTurn { get; set; }
    public List<Guid> AttackedCardIdsInCurrentTurn { get; set; } = [];
    public TurnDto CurrentTurn { get; set; } = null!;
    public List<PlayerGameDto> Players { get; set; } = [];
    public List<GameCardDto> Cards { get; set; } = [];
}
