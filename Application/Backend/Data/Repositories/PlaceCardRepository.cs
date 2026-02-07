using Backend.Data.Context;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class PlaceCardRepository(DuelNexusDbContext context) : Repository<PlaceCardAction>(context), IPlaceCardRepository
{
    public Task PlayCardActionAsync(GameCard gameCard, Turn turn, int? fieldIndex, bool faceDown, bool ddefensePosition, PlaceType type)
    {
        var placeCardAction = new PlaceCardAction
        {
            Card = gameCard,
            Turn = turn,
            FieldIndex = fieldIndex == -1 ? null : fieldIndex,
            FaceDown = faceDown,
            DefensePosition = ddefensePosition,
            Type = type
        };

        return AddAsync(placeCardAction);
    }

    public Task<PlaceCardAction?> GetLatestByTurnAndCardAsync(Guid turnId, Guid gameCardId)
    {
        return _dbSet
            .Where(p => p.TurnId == turnId && p.GameCardId == gameCardId)
            .OrderByDescending(p => p.ExecutedAt)
            .Include(p => p.Turn)
            .Include(p => p.Card).ThenInclude(gc => gc.Card)
            .FirstOrDefaultAsync();
    }

    public Task<PlaceCardAction?> GetByIdWithIncludesAsync(Guid id)
    {
        return _dbSet
            .Where(p => p.Id == id)
            .Include(p => p.Turn)
            .Include(p => p.Card).ThenInclude(gc => gc.Card)
            .FirstOrDefaultAsync();
    }
}
