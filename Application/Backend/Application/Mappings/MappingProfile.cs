using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Data.Models;
using Backend.Data.Enums;

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
        
    }
}
