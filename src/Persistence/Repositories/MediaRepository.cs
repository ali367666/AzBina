using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class MediaRepository
    : GenericRepository<MediaProperty, int>, IMediaPropertyRepository
{
    private readonly BinaDbContext _context;

    public MediaRepository(BinaDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<int> CountByPropertyListingIdAsync(int propertyListingId, CancellationToken ct = default)
    {
        return await _context.MediaProperties
            .CountAsync(m => m.PropertyListingId == propertyListingId, ct);
    }

    public async Task<int> GetMaxOrderByPropertyListingIdAsync(int propertyListingId, CancellationToken ct = default)
    {
        return await _context.MediaProperties
            .Where(m => m.PropertyListingId == propertyListingId)
            .Select(m => (int?)m.Order)
            .MaxAsync(ct) ?? 0;
    }

}
