using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PropertyListeningRepository
    : GenericRepository<PropertyListing, int>, IPropertyListeningRepository
{
    private readonly BinaDbContext _context;

    public PropertyListeningRepository(BinaDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.PropertyListings.AnyAsync(p => p.Id == id, ct);
    }
}
