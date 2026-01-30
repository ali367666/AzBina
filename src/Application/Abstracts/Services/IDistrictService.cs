using Application.DTOs.DistrictDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;

namespace Application.Abstracts.Services;

public interface IDistrictService
{
    Task <BaseResponse> CreateDistrictAsync(DistrictCreateDTO dto, CancellationToken ct = default);

    Task<List<GetAllDistrict>> GetAllDistrictAsync(CancellationToken ct = default);

    Task<GetByIdDistrict?> GetByIdDistrictAsync(int id, CancellationToken ct = default);
    Task<DistrictCreateDTO> DeleteDistrictAsync(int id, CancellationToken ct = default);
    Task<DistrictCreateDTO> UpdateDistrictAsync(int id, DistrictCreateDTO dto, CancellationToken ct = default);

}
