using Application.DTOs.DistrictDTOs.RequestDTOs;
using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface IDistrictRepository: IRepository<District,int>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default);
    Task<string?> GetNameByIdAsync(int id, CancellationToken ct = default);

}
