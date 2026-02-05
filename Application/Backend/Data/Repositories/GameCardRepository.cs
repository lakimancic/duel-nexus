using Backend.Data.Context;
using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameCardRepository(DuelNexusDbContext context) : Repository<GameCard>(context), IGameCardRepository
{
    public Task CreateGameCardsAsync(PlayerGame pg, List<DeckCard> deckCards)
    {
        var gameCards = new List<GameCard>();

        foreach (var deckCard in deckCards)
        {
            for (int i = 0; i < deckCard.Quantity; i++)
            {
                gameCards.Add(new GameCard
                {
                    PlayerGameId = pg.Id,
                    CardId = deckCard.CardId,
                    Zone = CardZone.Deck
                });
            }
        }

        gameCards.Shuffle();
        for (int i = 0; i < gameCards.Count; i++)
        {
            gameCards[i].DeckOrder = i;
        }

        return _context.Set<GameCard>().AddRangeAsync(gameCards);
    }

    public async Task<List<GameCard>> GetByPlayerGameIdAsync(Guid id)
    {
        return await _context.GameCards.Where(gc => gc.PlayerGameId == id).ToListAsync();
    }

    public async Task<GameCard?> GetByPlayerIdAndCardIdAsync(Guid playerId, Guid cardId)
    {
        return await _context.GameCards
            .Where(gc => gc.PlayerGameId == playerId && gc.CardId == cardId)
            .FirstOrDefaultAsync();
    }
}
