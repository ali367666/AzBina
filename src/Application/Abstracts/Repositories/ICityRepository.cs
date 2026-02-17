using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface ICityRepository:IRepository<City,int>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    
    Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default);
    Task<City?> GetByIdWithDistrictsAsync(int id, CancellationToken ct = default);
    Task<string?> GetNameByIdAsync(int id, CancellationToken ct = default);

}
