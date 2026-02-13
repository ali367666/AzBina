using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Options;
using Domain.Entities;
using Domain.Entities.Details;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Persistence.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _repo;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenService(IRefreshTokenRepository repo, IOptions<JwtOptions> jwtOptions)
    {
        _repo = repo;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<string> CreateAsync(User user, CancellationToken ct = default)
    {
        var token = GenerateSecureTokenHex(32);

        var entity = new RefreshToken
        {
            Token = token,
            UserId = user.Id,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshExpirationMinutes)
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);

        return token;
    }

    public async Task<User?> ValidateAndConsumeAsync(string token, CancellationToken ct = default)
    {
        var rt = await _repo.GetByTokenAsync(token, ct);
        if (rt is null)
            return null;

        if (rt.ExpiresAtUtc <= DateTime.UtcNow)
        {
            await _repo.DeleteByTokenAsync(token, ct);
            await _repo.SaveChangesAsync(ct);
            return null;
        }

        var user = rt.User;

        await _repo.DeleteByTokenAsync(token, ct);
        await _repo.SaveChangesAsync(ct);

        return user;
    }

    private static string GenerateSecureTokenHex(int byteLength)
    {
        var bytes = new byte[byteLength];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes);
    }
}
