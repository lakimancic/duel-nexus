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
    }
}
