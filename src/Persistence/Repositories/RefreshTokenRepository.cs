using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Domain.Entities;
using Domain.Entities.Details;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly BinaDbContext _db;

    public RefreshTokenRepository(BinaDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await _db.RefreshTokens.AddAsync(refreshToken, ct);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return await _db.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Token == token, ct);
    }

    public async Task<bool> DeleteByTokenAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var entity = await _db.RefreshTokens
            .SingleOrDefaultAsync(x => x.Token == token, ct);

        if (entity is null)
            return false;

        _db.RefreshTokens.Remove(entity);
        return true;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
