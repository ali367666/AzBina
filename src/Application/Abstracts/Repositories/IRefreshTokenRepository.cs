using Domain.Entities;
using Domain.Entities.Details;

namespace Application.Abstracts.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);

    Task<bool> DeleteByTokenAsync(string token, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
