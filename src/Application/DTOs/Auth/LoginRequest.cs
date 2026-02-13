namespace Application.DTOs.Auth;

public class LoginRequest
{
    public string Login { get; set; } = null!;  // Email və ya UserName
    public string Password { get; set; } = null!;

}
