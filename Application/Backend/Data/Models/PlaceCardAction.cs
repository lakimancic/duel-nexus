using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Data.Enums;

namespace Backend.Data.Models;

public class PlaceCardAction
{
    [Key]
    public Guid Id { get; set; }

    public Guid TurnId { get; set; }
    [ForeignKey(nameof(TurnId))]
    public Turn Turn { get; set; } = null!;

    public Guid PlayerGameId { get; set; }
    [ForeignKey(nameof(PlayerGameId))]
    public PlayerGame Player { get; set; } = null!;

    public Guid GameCardId { get; set; }
    [ForeignKey(nameof(GameCardId))]
    public GameCard Card { get; set; } = null!;

    public Guid CardFieldId { get; set; }
    [ForeignKey(nameof(CardFieldId))]
    public CardField Field { get; set; } = null!;

    public PlaceType Type { get; set; }

    public DateTime ExecutedAt { get; set; }
}
