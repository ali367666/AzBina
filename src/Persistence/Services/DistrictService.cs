using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using Application.Shared.Helpers;
using Application.Shared.Helpers.Responses;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Persistence.Repositories;

namespace Persistence.Services;

public class DistrictService:IDistrictService
{
    private readonly IDistrictRepository _districtRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<DistrictCreateDTO> _createDistrictValidator;
    public DistrictService(IDistrictRepository districtRepository,ICityRepository cityRepository,IMapper mapper,IValidator<DistrictCreateDTO> validator)
    {
        _districtRepository = districtRepository;
        _mapper = mapper;
        _cityRepository = cityRepository;
        _createDistrictValidator = validator;
    }

    public async Task<BaseResponse> CreateDistrictAsync(DistrictCreateDTO dto, CancellationToken ct = default)
    {
        await _createDistrictValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);
        var name = NameNormalizer.NormalizeName(dto.Name);


        var cityExists = await _cityRepository.ExistsByIdAsync(dto.CityId, ct);
        if (!cityExists)
            throw new KeyNotFoundException("Daxil etdiyiniz CityId mövcud deyil.");

        var exists = await _districtRepository.ExistsByNameAsync(name, ct);
        if (exists)
            throw new InvalidOperationException("Bu adda rayon artıq mövcuddur.");

        var district = _mapper.Map<District>(dto);
        district.Name = name;

        await _districtRepository.AddAsync(district, ct);
        await _districtRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok("District yaradıldı.");

    }


    public async Task<DistrictCreateDTO> DeleteDistrictAsync(int id, CancellationToken ct = default)
    {
        var district = await _districtRepository.GetByIdAsync(id, ct);

        if (district == null)
            throw new ArgumentException("District tapılmadı.");

        _districtRepository.Delete(district);
        await _districtRepository.SaveChangesAsync(ct);

        return new DistrictCreateDTO
        {
            Name = district.Name,
            CityId = district.CityId
        };
    }

    public async Task<List<GetAllDistrict>> GetAllDistrictAsync(CancellationToken ct = default)
    {
        var districts = await _districtRepository.GetAllAsync(ct);

        return districts
            .Select(d => new GetAllDistrict
            {
                Name = d.Name,
                CityId = d.CityId
            })
            .ToList();
    }

    public async Task<GetByIdDistrict?> GetByIdDistrictAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new ArgumentException("Id düzgün deyil.");

        var district = await _districtRepository    .GetByIdAsync(id, ct);

        if (district == null)
            return null;

        return new GetByIdDistrict
        {
            Id = district.Id,
            Name = district.Name
        };
    }

    public async Task<DistrictCreateDTO> UpdateDistrictAsync(int id, DistrictCreateDTO dto, CancellationToken ct = default)
    {
        var district = await _districtRepository.GetByIdAsync(id, ct);

        if (district == null)
            throw new ArgumentException("District tapılmadı.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("District name boş ola bilməz.");

        district.Name = dto.Name.Trim();

        _districtRepository.Update(district);
        await _districtRepository.SaveChangesAsync(ct);

        return dto;
    }
}
