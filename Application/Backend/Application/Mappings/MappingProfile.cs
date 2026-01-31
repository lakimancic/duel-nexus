using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Data.Models;
using Backend.Data.Enums;
using Backend.Application.DTOs.Decks;
<<<<<<< Updated upstream
using Backend.Application.DTOs.Player;
=======
using Backend.Application.DTOs.GameRooms;
using Backend.Application.DTOs.Users;
>>>>>>> Stashed changes

namespace Backend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, User>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, ShortUserDto>().ReverseMap();

        CreateMap<CardDto, MonsterCard>();
        CreateMap<CardDto, SpellCard>();
        CreateMap<CardDto, TrapCard>();


        CreateMap<MonsterCard, CardDto>();
        CreateMap<SpellCard, CardDto>();
        CreateMap<TrapCard, CardDto>();

        CreateMap<CardDto, Card>()
        .ConvertUsing((src, _, context) =>
        src.Type switch
        {
            CardType.Monster => context.Mapper.Map<MonsterCard>(src),
            CardType.Spell   => context.Mapper.Map<SpellCard>(src),
            CardType.Trap    => context.Mapper.Map<TrapCard>(src),
            _ => throw new ArgumentOutOfRangeException(nameof(src.Type))
        });

        CreateMap<InsertDeckCardDto, DeckCard>().ReverseMap();
        CreateMap<DeckCard, InsertDeckCardDto>().ReverseMap();

        CreateMap<DeckDto, Deck>().ReverseMap();
        CreateMap<Deck, DeckDto>().ReverseMap();

        CreateMap<GameRoom, GameRoomDto>().ReverseMap();
        CreateMap<GameRoomPlayer, GameRoomPlayerDto>()
        .ForMember(dest => dest.Id, opt =>
            opt.MapFrom(src => src.User != null ? src.User.Id : Guid.Empty))
        .ForMember(dest => dest.Username, opt =>
            opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))
        .ForMember(dest => dest.Elo, opt =>
            opt.MapFrom(src => src.User != null ? src.User.Elo : 0))
        .ForMember(dest => dest.IsReady, opt => opt.MapFrom(src => src.IsReady));

        CreateMap<EffectDto, Effect>().ReverseMap();
        CreateMap<Effect, EffectDto>().ReverseMap();
        CreateMap<CreateEffectDto, Effect>().ReverseMap();
        CreateMap<Effect, CreateEffectDto>().ReverseMap();


        CreateMap<PlayerCardDto, PlayerCard>().ReverseMap();
        CreateMap<PlayerCard, PlayerCardDto>().ReverseMap();
        CreateMap<CreatePlayerCardDto, PlayerCard>().ReverseMap();
        CreateMap<PlayerCard, CreatePlayerCardDto>().ReverseMap();

    }

}
