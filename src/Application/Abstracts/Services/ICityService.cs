using Application.Abstracts.Repositories;
using Application.DTOs.CityDTOs.RequestDTOs;

namespace Application.Abstracts.Services;

public interface ICityService
{
    Task<CreateCityDTOs> CreateCityAsync(CreateCityDTOs dto, CancellationToken ct = default);

    Task<List<GetAllCityDTOs>> GetAllCityAsync(CancellationToken ct = default);

    Task<GetByIdDTOs?> GetByIdCityAsync(int id, CancellationToken ct = default);
    Task<CreateCityDTOs>DeleteByIdCityAsync(int id,CancellationToken ct=default);
    Task<CreateCityDTOs>UpdateCityAsync(int id,CreateCityDTOs dto,CancellationToken ct=default);


}
