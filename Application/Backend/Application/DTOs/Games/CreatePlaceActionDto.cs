using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class CreatePlaceActionDto
{
    public Guid TurnId { get; set; }
    public Guid GameCardId { get; set; }
    public int? FieldIndex { get; set; }
    public bool FaceDown { get; set; }
    public bool DefensePosition { get; set; }
    public PlaceType Type { get; set; }
}
