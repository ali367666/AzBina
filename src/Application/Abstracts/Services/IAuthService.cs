using Application.DTOs.Auth;

namespace Application.Abstracts.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct = default);

    Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<bool> ConfirmEmailAsync(int userId, string token, CancellationToken ct = default);

}
