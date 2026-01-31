using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.CityDTOs.ResponseDTOs;
using Application.Shared.Helpers.Responses;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Persistence.Services;

public class CityService: ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCityDTOs> _createCityValidator;
    public CityService(ICityRepository cityRepository,IMapper mapper,IValidator<CreateCityDTOs> validator)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
        _createCityValidator = validator;
    }

    public async Task<BaseResponse> CreateCityAsync(CreateCityDTOs dto, CancellationToken ct = default)
    {
        await _createCityValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);
        var name = dto.Name!.Trim();

        var exists = await _cityRepository.ExistsByNameAsync(name, ct);
        if (exists)
            return BaseResponse.Fail("Bu adda şəhər artıq mövcuddur.");

        var city = _mapper.Map<City>(dto);
        city.Name = name;

        await _cityRepository.AddAsync(city, ct);
        await _cityRepository.SaveChangesAsync(ct);

        return BaseResponse.Ok("City yaradıldı.");
    }

    public async Task<BaseResponse<List<GetAllCityDTOs>>> GetAllCityAsync(CancellationToken ct = default)
    {

        var cities = await _cityRepository.GetAllAsync(ct);

        var data = _mapper.Map<List<GetAllCityDTOs>>(cities);

        return BaseResponse<List<GetAllCityDTOs>>.Ok(data, "City-lər gətirildi.");
    }
    public async Task<BaseResponse<GetByIdDTOs?>> GetByIdCityAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse<GetByIdDTOs?>.Fail("Id düzgün deyil.");

        var city = await _cityRepository.GetByIdAsync(id, ct);
        if (city == null)
            return BaseResponse<GetByIdDTOs?>.Fail($"Id={id} olan City tapılmadı.");

        var data = _mapper.Map<GetByIdDTOs>(city);

        return BaseResponse<GetByIdDTOs?>.Ok(data, "City tapıldı.");
    }
    public async Task<BaseResponse> UpdateCityAsync(
        int id,
        CreateCityDTOs dto,
        CancellationToken ct = default)
    {
        await _createCityValidator.ValidateAndThrowAsync(dto, cancellationToken: ct);
        if (id <= 0)
            return BaseResponse.Fail("Id düzgün deyil.");

        if (dto == null)
            return BaseResponse.Fail("DTO boş ola bilməz.");

        var city = await _cityRepository.GetByIdAsync(id, ct);
        if (city == null)
            return BaseResponse.Fail($"Id={id} olan City tapılmadı.");

        city.Name = dto.Name.Trim();
        _cityRepository.Update(city);
        await _cityRepository.SaveChangesAsync(ct);
        return BaseResponse.Ok("City yeniləndi.");

    }
    public async Task<BaseResponse> DeleteByIdCityAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse.Fail("Id düzgün deyil.");

        var city = await _cityRepository.GetByIdAsync(id, ct);
        if (city == null)
            return BaseResponse.Fail($"Id={id} olan City tapılmadı.");

        _cityRepository.Delete(city);
    await _cityRepository.SaveChangesAsync(ct);

    return BaseResponse.Ok("City silindi.");
    }


    public async Task<BaseResponse<CityWithDistrictsResponseDTO>> GetByIdCityWithDistrictsAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            return BaseResponse<CityWithDistrictsResponseDTO>.Fail("Id düzgün deyil.");

        var city = await _cityRepository.GetByIdWithDistrictsAsync(id, ct);
        if (city is null)
            return BaseResponse<CityWithDistrictsResponseDTO>.Fail($"Id={id} olan City tapılmadı.");

        var data = new CityWithDistrictsResponseDTO
        {
            Id = city.Id,
            Name = city.Name,
            DistrictNames = city.Districts.Select(d => d.Name).ToList()
        };

        return BaseResponse<CityWithDistrictsResponseDTO>.Ok(data, "City və district-lər gətirildi.");
    }

}
