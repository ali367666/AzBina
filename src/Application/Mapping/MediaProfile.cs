using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.MediaPropertyDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // MEDIA: Entity -> DTO
        CreateMap<MediaProperty, GetByIdMediaProperty>();
        CreateMap<MediaProperty, GetAllMediaProperty>();

        // PROPERTY LISTING: Create DTO -> Entity
        CreateMap<CreatePropertyListing, PropertyListing>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title.Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description.Trim()));

        // PROPERTY LISTING: Entity -> DTO
        CreateMap<PropertyListing, GetByIdPropertyListing>();
        CreateMap<PropertyListing, GetAllPropertyListing>();

        // CITY: Entity -> DTO
        CreateMap<City, GetAllCityDTOs>();
        CreateMap<City, GetByIdDTOs>();

        // CITY: Create DTO -> Entity
        CreateMap<CreateCityDTOs, City>();
    }
}
