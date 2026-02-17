using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class DistrictRepository:GenericRepository<District,int>,IDistrictRepository
{
    private readonly BinaDbContext _context;
    public DistrictRepository(BinaDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Districts
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct);
    }
    public async Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Cities
            .AnyAsync(c => c.Id == id, ct);
    }
    public async Task<string?> GetNameByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Districts
            .Where(x => x.Id == id)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(ct);
    }
}
