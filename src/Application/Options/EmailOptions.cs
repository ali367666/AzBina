namespace Application.Options;

public class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>
    /// Ümumiyyətlə email göndərilsin?
    /// (Dev-də false edib sistemi rahat test etmək olar)
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// SMTP server parametrləri
    /// </summary>
    public SmtpOptions Smtp { get; set; } = new();

    /// <summary>
    /// Göndərən məlumatları
    /// </summary>
    public SenderOptions Sender { get; set; } = new();

    /// <summary>
    /// Email təsdiq linkinin əsas URL-i.
    /// Məs: https://localhost:5173  (frontend)
    /// və ya https://localhost:7273 (api)
    /// </summary>
    public string ConfirmBaseUrl { get; set; } = string.Empty;

    public sealed class SmtpOptions
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;

        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public sealed class SenderOptions
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
