using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.PropertyListeningDTOs.RequestDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class PropertyListingProfile:Profile
{
    public PropertyListingProfile()
    {
        CreateMap<CreatePropertyListing, PropertyListing>();

        // 🔹 GET ALL: Entity -> DTO
        CreateMap<PropertyListing, GetAllPropertyListing>();

        // 🔹 GET BY ID: Entity -> DTO
        CreateMap<PropertyListing, GetByIdPropertyListing>();

        // (İstəyə görə)
        CreateMap<PropertyListing, CreatePropertyListing>();

    }

}
