using Application.DTOs.DistrictDTOs.RequestDTOs;
using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface IDistrictRepository: IRepository<District,int>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);

}
