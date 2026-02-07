namespace Backend.Application.DTOs.Games;

public class AttackActionDto
{
    public Guid Id { get; set; }
    public ShortTurnDto Turn { get; set; } = null!;
    public GameCardDto Attacker { get; set; } = null!;
    public GameCardDto? Defender { get; set; }
    public PlayerGameDto? DefenderPlayer { get; set; }
    public DateTime ExecutedAt { get; set; }
}
