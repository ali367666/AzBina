using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CityRepository: GenericRepository<City, int>, ICityRepository
{
    private readonly BinaDbContext _context;    
    public CityRepository(BinaDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Cities
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct);
    }
    public async Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Cities
            .AnyAsync(c => c.Id == id, ct);
    }
}
