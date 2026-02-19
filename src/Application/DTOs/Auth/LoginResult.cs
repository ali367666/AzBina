namespace Application.DTOs.Auth;

public sealed class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int FailedAttemptCount { get; set; }
    public int RemainingAttemptsToLockout { get; set; }
    public TokenResponse? Token { get; set; }
}
