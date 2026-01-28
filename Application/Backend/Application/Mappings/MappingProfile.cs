using AutoMapper;
using Backend.Application.DTOs.Auth;
using Backend.Data.Models;

namespace Backend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, User>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<CardDto, Card>().ReverseMap();
        CreateMap<Card, CardDto>().ReverseMap();

        CreateMap<DeckCardDto, DeckCard>().ReverseMap();
        CreateMap<DeckCard, DeckCardDto>().ReverseMap();

        CreateMap<DeckDto, Deck>().ReverseMap();
        CreateMap<Deck, DeckDto>().ReverseMap();
        
    }
}
