using Application.DTOs.DistrictDTOs.RequestDTOs;

namespace Application.Abstracts.Services;

public interface IDistrictService
{
    Task<DistrictCreateDTO> CreateDistrictAsync(DistrictCreateDTO dto, CancellationToken ct = default);

    Task<List<GetAllDistrict>> GetAllDistrictAsync(CancellationToken ct = default);

    Task<GetByIdDistrict?> GetByIdDistrictAsync(int id, CancellationToken ct = default);

}
