namespace Backend.Application.DTOs.Games;

public class PlayCardDto
{
    public Guid CardId { get; set; }
    public int FieldIndex { get; set; }
    public bool FaceDown { get; set; }
    public bool DefensePosition { get; set; }
}
