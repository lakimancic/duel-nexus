namespace Backend.Application.DTOs.Games;

public class GameResultDto
{
    public Guid GameId { get; set; }
    public Guid RoomId { get; set; }
    public bool IsRanked { get; set; }
    public DateTime FinishedAt { get; set; }
    public Guid? WinnerPlayerGameId { get; set; }
    public Guid? WinnerUserId { get; set; }
    public string? WinnerUsername { get; set; }
    public List<GameEndPlayerRatingChangeDto> PlayerRatingChanges { get; set; } = [];
}

public class GameEndPlayerRatingChangeDto
{
    public Guid PlayerGameId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public double OldElo { get; set; }
    public double NewElo { get; set; }
    public double Delta { get; set; }
    public bool IsWinner { get; set; }
}
