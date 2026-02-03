using Backend.Data.Enums;
using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IPlaceCardRepository : IRepository<PlaceCardAction>
{
    Task PlayCardActionAsync(GameCard gameCard, Turn turn, int fieldIndex, bool faceDown, bool ddefensePosition, PlaceType type);
}
