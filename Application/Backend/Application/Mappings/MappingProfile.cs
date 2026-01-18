using AutoMapper;
using Backend.Domain.Entities;
using Backend.Infrastructure.Persistence.Entities;

namespace Backend.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserEntity, User>().ReverseMap();
    }
}