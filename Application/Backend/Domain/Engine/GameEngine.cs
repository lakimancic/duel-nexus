namespace Backend.Domain.Engine;

using Backend.Data.Enums;
using Backend.Data.Models;
using Backend.Data.UnitOfWork;
using Backend.Domain.Commands;
using Backend.Utils.WebApi;
using Microsoft.Extensions.DependencyInjection;

public sealed class GameEngine(
    IUnitOfWork unitOfWork,
    IGameCommandLock commandLock,
    IServiceProvider serviceProvider) : IGameEngine
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGameCommandLock _commandLock = commandLock;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task InitializeGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        var players = await _unitOfWork.PlayerGames.GetByGameIdOrderedAsync(game.Id);
        if (players.Count == 0)
            throw new BadRequestException("Game has no players.");

        var currentTurn = await _unitOfWork.Turns.GetCurrentTurnAsync(game.Id)
            ?? throw new BadRequestException("Game turn was not initialized.");

        currentTurn.Phase = TurnPhase.Draw;
        _unitOfWork.Turns.Update(currentTurn);

        foreach (var player in players)
        {
            player.TurnEnded = false;
            _unitOfWork.PlayerGames.Update(player);
            await DrawStartingHandAsync(player.Id, cancellationToken);
        }
    }

    public async Task<TResult> ExecuteCommandAsync<TCommand, TResult>(Guid gameId, Guid userId, TCommand command, CancellationToken cancellationToken = default)
        where TCommand : IGameCommand<TResult>
    {
        return await _commandLock.ExecuteAsync(gameId, async () =>
        {
            var game = await GetGameOrThrow(gameId, cancellationToken);
            var currentTurn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
                ?? throw new ObjectNotFoundException("Turn not found");

            var actor = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(userId, gameId)
                ?? throw new BadRequestException("Player is not part of this game.");

            var commandContext = new GameCommandContext(
                UnitOfWork: _unitOfWork,
                Game: game,
                CurrentTurn: currentTurn,
                Actor: actor
            );

            var handler = _serviceProvider.GetRequiredService<IGameCommandHandler<TCommand, TResult>>();
            var result = await handler.HandleAsync(command, commandContext, cancellationToken);
            await _unitOfWork.CompleteAsync();
            return result;
        });
    }

    public async Task<GameStateSnapshot> GetGameStateAsync(Guid gameId, Guid viewerUserId, CancellationToken cancellationToken = default)
    {
        var game = await GetGameOrThrow(gameId, cancellationToken);

        var viewer = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(viewerUserId, gameId)
            ?? throw new BadRequestException("Player is not part of this game.");

        var currentTurn = await _unitOfWork.Turns.GetCurrentTurnAsync(gameId)
            ?? throw new ObjectNotFoundException("Turn not found");

        var players = await _unitOfWork.PlayerGames.GetByGameIdWithUserAsync(gameId);
        var cards = await _unitOfWork.GameCards.GetByGameIdWithCardAsync(gameId);

        return new GameStateSnapshot(game, currentTurn, viewer, players, cards);
    }

    public async Task<bool> UserExistsInGameAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default)
    {
        var player = await _unitOfWork.PlayerGames.GetByUserIdAndGameIdAsync(userId, gameId);
        return player != null;
    }

    private async Task DrawStartingHandAsync(Guid playerGameId, CancellationToken cancellationToken)
    {
        var startingCards = await _unitOfWork.GameCards.GetTopDeckCardsByPlayerAsync(playerGameId, GameConstants.StartingHandSize);

        foreach (var card in startingCards)
        {
            card.Zone = CardZone.Hand;
            card.DeckOrder = null;
            _unitOfWork.GameCards.Update(card);
        }
    }

    private async Task<Game> GetGameOrThrow(Guid gameId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Games.GetByIdAsync(gameId)
            ?? throw new ObjectNotFoundException("Game not found");
    }
}
