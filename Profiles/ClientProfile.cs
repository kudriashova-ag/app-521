using AutoMapper;
using myApp.DTOs.Clients;
using MyApp.Models;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        // TSource -> TDestination
        CreateMap<Client, ClientReadDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));


        CreateMap<ClientCreateDto, Client>();
    }
}