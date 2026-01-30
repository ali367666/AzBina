using Application.DTOs.CityDTOs.RequestDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class CityProfile:Profile
{
    public CityProfile()
    {
        CreateMap<CreateCityDTOs, City>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()));

        // 🔹 GET ALL: Entity -> DTO
        CreateMap<City, GetAllCityDTOs>();

        // 🔹 GET BY ID: Entity -> DTO
        CreateMap<City, GetByIdDTOs>();

        // (İstəyə görə)
        CreateMap<City, CreateCityDTOs>();
    }
}
