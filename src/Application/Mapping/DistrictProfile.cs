using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class DistrictProfile:Profile
{
    public DistrictProfile()
    {
        // Request DTO -> Entity
        CreateMap<DistrictCreateDTO, District>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()));

        // Entity -> Response DTO
        CreateMap<District, DistrictCreateDTO>();
    }

}
