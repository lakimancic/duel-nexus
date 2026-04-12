namespace Backend.Application.DTOs.Users;

public class ScoreboardEntryDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public double Elo { get; set; }
    public int Rank { get; set; }
}

public class UserProfileStatsDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public double Elo { get; set; }
    public List<UserProfileGameDto> Games { get; set; } = [];
}

public class UserProfileGameDto
{
    public Guid GameId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public bool IsRanked { get; set; }
    public int? Placement { get; set; }
    public bool EloChangeAvailable { get; set; }
    public List<UserProfileGamePlayerDto> Players { get; set; } = [];
}

public class UserProfileGamePlayerDto
{
    public Guid PlayerGameId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public int Placement { get; set; }
    public int LifePoints { get; set; }
    public bool IsWinner { get; set; }
    public double? OldElo { get; set; }
    public double? NewElo { get; set; }
    public double? Delta { get; set; }
}
