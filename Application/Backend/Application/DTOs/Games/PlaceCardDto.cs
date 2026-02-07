using Backend.Data.Enums;

namespace Backend.Application.DTOs.Games;

public class PlaceCardDto
{
    public ShortTurnDto Turn { get; set; } = null!;
    public GameCardDto GameCard { get; set; } = null!;
    public int? FieldIndex { get; set; }
    public bool FaceDown { get; set; }
    public bool DefensePosition { get; set; }
    public PlaceType Type { get; set; }
}
