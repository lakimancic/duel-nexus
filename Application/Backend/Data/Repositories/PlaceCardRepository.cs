using Backend.Data.Context;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class PlaceCardRepository(DuelNexusDbContext context) : Repository<PlaceCardAction>(context), IPlaceCardRepository
{
    public Task PlayCardActionAsync(GameCard gameCard, Turn turn, int fieldIndex, bool faceDown, bool ddefensePosition, PlaceType type)
    {
        var placeCardAction = new PlaceCardAction
        {
            Card = gameCard,
            Turn = turn,
            FieldIndex = fieldIndex,
            FaceDown = faceDown,
            DefensePosition = ddefensePosition,
            Type = type
        };

        return AddAsync(placeCardAction);
    }
}
