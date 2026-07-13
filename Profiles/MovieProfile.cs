using AutoMapper;
using myApp.DTOs.Movies;
using MyApp.Models;

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<Movie, MovieReadDto>();
        // 
        CreateMap<MovieActor, MovieCastMemberDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Actor.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Actor.FirstName} {src.Actor.LastName}"));
        
        CreateMap<Movie, MovieDetailDto>()
            .ForMember(dest => dest.Actors, opt=> opt.MapFrom(src => src.MovieActors));
        
        CreateMap<MovieCreateDto, Movie>();
    }
}
