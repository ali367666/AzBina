using Domain.Entities;

namespace Application.Abstracts.Services;

public interface IRefreshTokenService
{
    /// <summary>
    /// User üçün yeni refresh token yaradır, DB-də saxlayır və token string qaytarır.
    /// </summary>
    Task<string> CreateAsync(User user, CancellationToken ct = default);

    /// <summary>
    /// Verilən refresh tokeni DB-də tapır.
    /// Əgər mövcuddursa və vaxtı keçməyibsə -> "consume" edir (silir) və User qaytarır.
    /// Əks halda null.
    /// </summary>
    Task<User?> ValidateAndConsumeAsync(string token, CancellationToken ct = default);
}
