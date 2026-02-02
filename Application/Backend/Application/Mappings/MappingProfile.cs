using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Data.Models;
using Backend.Data.Enums;
using Backend.Application.DTOs.Decks;
using Backend.Application.DTOs.Player;
using Backend.Application.DTOs.GameRooms;
using Backend.Application.DTOs.Users;
using Backend.Application.DTOs.Effects;
using Backend.Utils.Data;
using Backend.Application.DTOs.Chat;

namespace Backend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, User>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, ShortUserDto>().ReverseMap();

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

        CreateMap<InsertDeckCardDto, DeckCard>().ReverseMap();

        CreateMap<DeckDto, Deck>().ReverseMap();

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
        CreateMap<CreateEffectDto, Effect>().ReverseMap();

        CreateMap<PlayerCardDto, PlayerCard>().ReverseMap();
        CreateMap<CreatePlayerCardDto, PlayerCard>().ReverseMap();

        CreateMap<ChatMessage, ChatMessageDto>();
        CreateMap<SendMessageDto, ChatMessage>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.Global));
        CreateMap<GameRoomMessageDto, ChatMessage>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.GameRoom));
        CreateMap<PrivateMessageDto, ChatMessage>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MessageType.Private));

        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>))
            .ConvertUsing(typeof(PagedResultConverter<,>));
    }

}
