using Application.Abstracts.Repositories;
using Application.DTOs.CityDTOs.RequestDTOs;
using Application.Shared.Helpers.Responses;

namespace Application.Abstracts.Services;

public interface ICityService
{
    Task<BaseResponse> CreateCityAsync(CreateCityDTOs dto, CancellationToken ct = default);

    Task<BaseResponse<List<GetAllCityDTOs>>> GetAllCityAsync(CancellationToken ct = default);

    Task<BaseResponse<GetByIdDTOs?>> GetByIdCityAsync(int id, CancellationToken ct = default);
    Task<BaseResponse>DeleteByIdCityAsync(int id,CancellationToken ct=default);
    Task<BaseResponse>UpdateCityAsync(int id,CreateCityDTOs dto,CancellationToken ct=default);
    Task GetByIdCityWithDistrictsAsync(int id, CancellationToken ct);
}
