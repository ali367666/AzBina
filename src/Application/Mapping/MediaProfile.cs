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
        // =========================
        // MEDIA: Create DTO -> Entity
        // =========================
        CreateMap<CreateMediaProperty, MediaProperty>()
            .ForMember(d => d.MediaUrl, o => o.MapFrom(s => s.MediaUrl.Trim()))
            .ForMember(d => d.MediaType, o => o.MapFrom(s => s.MediaType.Trim().ToLower()));

        // =========================
        // MEDIA: Entity -> DTO (əgər bu DTO-ları istifadə edirsənsə)
        // =========================
        CreateMap<MediaProperty, GetByIdMediaProperty>();
        CreateMap<MediaProperty, GetAllMediaProperty>();

        // =========================
        // PROPERTY LISTING: Create DTO -> Entity (əgər servisdə mapper edirsənsə)
        // =========================
        CreateMap<CreatePropertyListing, PropertyListing>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title.Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description.Trim()));

        // PROPERTY LISTING: Entity -> DTO
        CreateMap<PropertyListing, GetByIdPropertyListing>();
        CreateMap<PropertyListing, GetAllPropertyListing>();

        // =========================
        // CITY: Entity -> DTO
        // =========================
        CreateMap<City, GetAllCityDTOs>();
        CreateMap<City, GetByIdDTOs>();

        // (İstəyə görə) CreateCityDTOs -> City (Create üçün adətən bu lazımdır)
        CreateMap<CreateCityDTOs, City>();
    }
}
