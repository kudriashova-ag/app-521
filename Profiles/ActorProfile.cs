using AutoMapper;
using myApp.DTOs.Actors;
using MyApp.Models;

public class ActorMappingProfile : Profile
{
    public ActorMappingProfile()
    {
        CreateMap<Actor, ActorReadDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        CreateMap<ActorCreateDto, Actor>();
    }
}
