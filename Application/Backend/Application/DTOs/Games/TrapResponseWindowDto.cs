namespace Backend.Application.DTOs.Games;

public class TrapResponseWindowDto
{
    public Guid GameId { get; set; }
    public Guid DefenderPlayerGameId { get; set; }
    public Guid DefenderUserId { get; set; }
    public int TimeoutSeconds { get; set; }
    public List<TrapCardOptionDto> AvailableTrapCards { get; set; } = [];
}
