using AutoMapper;
using myApp.DTOs.Directors;
using MyApp.Models;

public class DirectorMappingProfile : Profile
{
    public DirectorMappingProfile()
    {
        CreateMap<Director, DirectorReadDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        CreateMap<DirectorCreateDto, Director>();
    }
}
