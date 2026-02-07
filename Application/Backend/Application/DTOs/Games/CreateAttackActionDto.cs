namespace Backend.Application.DTOs.Games;

public class CreateAttackActionDto
{
    public Guid TurnId { get; set; }
    public Guid AttackerCardId { get; set; }
    public Guid? DefenderCardId { get; set; }
    public Guid? DefenderPlayerGameId { get; set; }
}
