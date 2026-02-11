using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Data.Models;
using Backend.Data.Enums;
using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.GameRooms;
using Backend.Application.DTOs.Users;
using Backend.Application.DTOs.Effects;
using Backend.Utils.Data;
using Backend.Application.DTOs.Chat;
using Backend.Application.DTOs.Cards;
using Backend.Application.DTOs.Games;

namespace Backend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Users
        CreateMap<RegisterDto, User>();
        CreateMap<EditUserDto, User>();
        CreateMap<User, UserDto>();
        CreateMap<User, ShortUserDto>();

        // Cards
        CreateMap<CreateCardDto, MonsterCard>();
        CreateMap<CreateCardDto, SpellCard>();
        CreateMap<CreateCardDto, TrapCard>();

        CreateMap<MonsterCard, CardDto>();
        CreateMap<SpellCard, CardDto>();
        CreateMap<TrapCard, CardDto>();

        CreateMap<CreateCardDto, Card>()
        .ConvertUsing((src, _, context) =>
        src.Type switch
        {
            CardType.Monster => context.Mapper.Map<MonsterCard>(src),
            CardType.Spell   => context.Mapper.Map<SpellCard>(src),
            CardType.Trap    => context.Mapper.Map<TrapCard>(src),
            _ => throw new ArgumentOutOfRangeException(nameof(src.Type))
        });

        // Decks
        CreateMap<CreateDeckDto, Deck>();
        CreateMap<Deck, DeckDto>();
        CreateMap<InsertDeckCardDto, DeckCard>();
        CreateMap<DeckCard, DeckCardDto>();
        CreateMap<EditDeckDto,Deck>();
        CreateMap<Deck, EditDeckDto>();

        // Effects
        CreateMap<CreateEffectDto, Effect>();
        CreateMap<Effect, EffectDto>();

        // PlayerCards
        CreateMap<CreatePlayerCardDto, PlayerCard>();
        CreateMap<PlayerCard, PlayerCardDto>();

        CreateMap<ChatMessage, ChatMessageDto>();
        CreateMap<SendMessageDto, ChatMessage>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.Global));
        CreateMap<GameRoomMessageDto, ChatMessage>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.GameRoom));
        CreateMap<PrivateMessageDto, ChatMessage>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.Private));

        // GameRooms
        CreateMap<CreateGameRoomDto, GameRoom>();
        CreateMap<EditGameRoomDto, GameRoom>();
        CreateMap<GameRoom, GameRoomDto>();

        CreateMap<GameRoomPlayer, GameRoomPlayerDto>();


        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>))
            .ConvertUsing(typeof(PagedResultConverter<,>));

        // Games
        CreateMap<Game, GameDto>();
        CreateMap<Game,EditGameDto>();
        CreateMap<EditGameDto, Game>();
        CreateMap<PlayerGame, PlayerGameDto>();
        CreateMap<PlayerGame, EditPlayerGameDto>();
        CreateMap<EditPlayerGameDto, PlayerGame>();
        CreateMap<GameCard, GameCardDto>();

        CreateMap<Turn, TurnDto>();
        CreateMap<Turn, ShortTurnDto>();

        CreateMap<CreateAttackActionDto, AttackAction>();
        CreateMap<AttackAction, AttackActionDto>();

        CreateMap<CreateCardMovementDto, CardMovementAction>();
        CreateMap<CardMovementAction, CardMovementDto>();

        CreateMap<CreatePlaceActionDto, PlaceCardAction>();
        CreateMap<PlaceCardAction, PlaceCardDto>();

        CreateMap<CreateEffectActivationDto, EffectActivation>();
        
        CreateMap<EffectActivation, EffectActivationDto>();

        CreateMap<EffectActivationDto, EffectActivation>();


        CreateMap<EditGameCardDto, GameCard>();
        CreateMap<EditTurnDto, Turn>();
    }

}
