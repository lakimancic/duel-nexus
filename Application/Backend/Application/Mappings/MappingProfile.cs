using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Data.Models;
using Backend.Data.Enums;
using Backend.Application.DTOs.Decks;

namespace Backend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, User>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();

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

        // CreateMap<CardDto, Card>().ReverseMap();
        // CreateMap<Card, CardDto>().ReverseMap();


        CreateMap<DeckCardDto, DeckCard>().ReverseMap();
        CreateMap<DeckCard, DeckCardDto>().ReverseMap();

        CreateMap<DeckDto, Deck>().ReverseMap();
        CreateMap<Deck, DeckDto>().ReverseMap();

        // Game rooms
        CreateMap<GameRoomPlayer, DTOs.GameRooms.GameRoomPlayerDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserElo, opt => opt.MapFrom(src => src.User.Elo))
            .ForMember(dest => dest.IsReady, opt => opt.MapFrom(src => src.IsReady));

        CreateMap<GameRoom, DTOs.GameRooms.GameRoomDto>()
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players));


        CreateMap<EffectDto, Effect>().ReverseMap();
        CreateMap<Effect, EffectDto>().ReverseMap();


        CreateMap<CreateEffectDto, Effect>().ReverseMap();
        CreateMap<Effect, CreateEffectDto>().ReverseMap();

    }

}
