using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.DTOs.DistrictDTOs.RequestDTOs;
using Persistence.Repositories;

namespace Persistence.Services;

public class DistrictService:IDistrictService
{
    private readonly IDistrictRepository _repo;
    public DistrictService(IDistrictRepository repo)
    {
        _repo = repo;
    }

    public async Task<DistrictCreateDTO> CreateDistrictAsync(DistrictCreateDTO dto, CancellationToken ct = default)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("District name boş ola bilməz.");

        var district = new Domain.Entities.District
        {
            Name = dto.Name.Trim(),
            CityId = dto.CityId
        };

        await _repo.AddAsync(district, ct);
        await _repo.SaveChangesAsync(ct);

        return dto;
    }


    public Task<List<GetAllDistrict>> GetAllDistrictAsync(CancellationToken ct = default)
    {
        var districts =  _repo.GetAllAsync(ct);
        return districts.ContinueWith(t => t.Result
            .Select(d => new GetAllDistrict
            {
                Name = d.Name,
                CityId = d.CityId
            })
            .ToList(), ct);
    }

    public async Task<GetByIdDistrict?> GetByIdDistrictAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0)
            throw new ArgumentException("Id düzgün deyil.");

        var district = await _repo.GetByIdAsync(id, ct);

        if (district == null)
            return null;

        return new GetByIdDistrict
        {
            Id = district.Id,
            Name = district.Name
        };
    }
    
}
