namespace Backend.Application.DTOs.Games;

public class AttackActionDto
{
    public Guid CardId { get; set; }
    public Guid? TargetCardId { get; set; }
    public Guid? TargetPlayerId { get; set; }
}
