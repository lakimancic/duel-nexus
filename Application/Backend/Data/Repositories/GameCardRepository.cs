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

        return _dbSet.AddRangeAsync(gameCards);
    }

    public async Task<GameCard?> GetByWithCardById(Guid id)
    {
        return await _dbSet
            .Where(gc => gc.Id == id)
            .Include(gc => gc.Card)
            .FirstOrDefaultAsync();
    }

    public Task<List<GameCard>> GetByGameIdWithCardAsync(Guid gameId)
    {
        return _dbSet
            .Where(gc => gc.PlayerGame.GameId == gameId)
            .Include(gc => gc.Card)
            .ToListAsync();
    }
}
