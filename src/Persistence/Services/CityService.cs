using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;

namespace Persistence.Services;

public class CityService: ICityService
{
    private readonly ICityRepository _cityRepository;
    public CityService(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<CreateCityDTOs> CreateCityAsync(CreateCityDTOs dto, CancellationToken ct = default)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name boş ola bilməz.");

        var city = new Domain.Entities.City
        {
            Name = dto.Name.Trim()
        };

        await _cityRepository.AddAsync(city, ct);
        await _cityRepository.SaveChangesAsync(ct);

        return dto; 
    }
    public async Task<List<GetAllCityDTOs>> GetAllCityAsync(CancellationToken ct = default)
    {
        var cities = await _cityRepository.GetAllAsync(ct);

        return cities
            .Select(c => new GetAllCityDTOs
            {
                Name = c.Name
            })
            .ToList();
    }
    public async Task<GetByIdDTOs?> GetByIdCityAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new ArgumentException("Id düzgün deyil.");

        var city = await _cityRepository.GetByIdAsync(id, ct);

        if (city == null)
            return null;

        return new GetByIdDTOs
        {
            Id = city.Id,
            Name = city.Name
        };
    }
    public async Task<CreateCityDTOs> UpdateCityAsync(
        int id,
        CreateCityDTOs dto,
        CancellationToken ct = default)
    {
        if (id <= 0)
            throw new ArgumentException("Id düzgün deyil.");

        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var city = await _cityRepository.GetByIdAsync(id, ct);
        if (city == null)
            throw new KeyNotFoundException($"Id={id} olan City tapılmadı.");

        city.Name = dto.Name.Trim();

        _cityRepository.Update(city);
        await _cityRepository.SaveChangesAsync(ct);
        return dto;
    }
    public async Task<CreateCityDTOs> DeleteByIdCityAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
            throw new ArgumentException("Id düzgün deyil.");

        var city = await _cityRepository.GetByIdAsync(id, ct);
        if (city == null)
            throw new KeyNotFoundException($"Id={id} olan City tapılmadı.");

        var deletedDto = new CreateCityDTOs
        {
            Name = city.Name
        };

        _cityRepository.Delete(city);
        await _cityRepository.SaveChangesAsync(ct);

        return deletedDto;
    }




}
