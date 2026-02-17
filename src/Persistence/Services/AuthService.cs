using System.Text;
using Application.Abstracts.Services;
using Application.DTOs.Auth;
using Application.Options;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Persistence.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenGenerator _jwtGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtOptions _jwtOptions;

    // ✅ Addım 5: email
    private readonly IEmailSender _emailSender;
    private readonly EmailOptions _emailOptions;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenGenerator jwtGenerator,
        IRefreshTokenService refreshTokenService,
        IOptions<JwtOptions> jwtOptions,
        IEmailSender emailSender,
        IOptions<EmailOptions> emailOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _refreshTokenService = refreshTokenService;
        _jwtOptions = jwtOptions.Value;

        _emailSender = emailSender;
        _emailOptions = emailOptions.Value;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            FullName = request.FullName,

            // ✅ Email təsdiqlənməlidir
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var message = string.Join(" | ", result.Errors.Select(e => e.Description));
            return (false, message);
        }

        // ✅ Default role
        var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.User);
        if (!roleResult.Succeeded)
        {
            var message = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
            return (false, message);
        }

        // ✅ Email təsdiq tokeni + link + email göndər
        // Email göndərmə söndürülübsə, sadəcə keç
        if (_emailOptions.Enabled && !string.IsNullOrWhiteSpace(user.Email))
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // URL-safe et
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Base URL-də sondakı slash-i götür
            var baseUrl = (_emailOptions.ConfirmBaseUrl ?? string.Empty).TrimEnd('/');

            // Confirm endpoint-in birbaşa vurulması üçün link
            // Sən endpoint-i belə edəcəksən: GET /api/auth/confirm-email?userId=..&token=..
            var confirmLink = $"{baseUrl}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

            var subject = "Email təsdiqi";
            var html = $@"
                <p>Qeydiyyatınızı tamamlamaq üçün zəhmət olmasa aşağıdakı linkə keçid edin:</p>
                <p><a href=""{confirmLink}"">{confirmLink}</a></p>
            ";

            var text = $"Qeydiyyatınızı tamamlamaq üçün bu linkə keçid edin: {confirmLink}";

            await _emailSender.SendAsync(user.Email, subject, html, text, ct);
        }

        return (true, null);
    }

    public async Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Login)
                   ?? await _userManager.FindByNameAsync(request.Login);

        if (user is null)
            return null;

        var check = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: false);

        if (!check.Succeeded)
            return null;

        // ✅ Email təsdiq yoxlaması
        if (!user.EmailConfirmed)
        {
            // Spec: aydın mesaj
            // Səndə LoginAsync TokenResponse? qaytardığı üçün burada "null" qaytarmaq
            // mesajı itirir. Daha yaxşısı: TokenResponse-a Error əlavə etmək və ya Result pattern.
            // Amma spec tələb edir deyə ən azından Exception da atmaq olar (controller tutub 400 versin).
            throw new InvalidOperationException("Email təsdiqlənməyib. Zəhmət olmasa emailinizə göndərilən təsdiq linkinə keçid edin.");
        }

        return await BuildTokenResponseAsync(user, ct);
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var user = await _refreshTokenService.ValidateAndConsumeAsync(refreshToken, ct);
        if (user is null)
            return null;

        return await BuildTokenResponseAsync(user, ct);
    }

    // ✅ Addım 4 interface-də əlavə etdiyin metodun implementasiyası
    public async Task<bool> ConfirmEmailAsync(int userId, string token, CancellationToken ct = default)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(token))
            return false;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return false;

        // Token URL-safe encode olunub, decode edirik
        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        return result.Succeeded;
    }

    private async Task<TokenResponse> BuildTokenResponseAsync(User user, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtGenerator.GenerateAccessToken(user, roles);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

        var refreshToken = await _refreshTokenService.CreateAsync(user, ct);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAtUtc = expiresAtUtc
        };
    }
}
