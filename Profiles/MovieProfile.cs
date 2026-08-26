using AutoMapper;
using myApp.DTOs.Movies;
using myApp.Models;
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

        CreateMap<MovieAttachment, MovieAttachmentDto>()
            .ForMember(dest => dest.DownloadUrl, opt => opt.Ignore())
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.OriginalFileName));

        CreateMap<Movie, MovieDetailDto>()
            .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.MovieActors));

        CreateMap<MovieCreateDto, Movie>();
    }
}
