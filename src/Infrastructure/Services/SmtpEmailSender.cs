using System.Net;
using System.Net.Mail;
using Application.Abstracts.Services;
using Application.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    public SmtpEmailSender(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        string? textBody = null,
        CancellationToken ct = default)
    {
        // 1) Guard checks
        if (!_options.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(toEmail))
            return;

        // Body seçimi: HTML varsa onu, yoxsa plain text
        var hasHtml = !string.IsNullOrWhiteSpace(htmlBody);
        var body = hasHtml ? htmlBody : (textBody ?? string.Empty);

        // Boş body göndərməyək
        if (string.IsNullOrWhiteSpace(body))
            return;

        // 2) Mail message
        var fromEmail = _options.Sender.Email;
        var fromName = _options.Sender.Name;

        if (string.IsNullOrWhiteSpace(fromEmail))
            throw new InvalidOperationException("Email sender address (Email:Sender:Email) is not configured.");

        using var message = new MailMessage
        {
            From = string.IsNullOrWhiteSpace(fromName)
                ? new MailAddress(fromEmail)
                : new MailAddress(fromEmail, fromName),
            Subject = subject ?? string.Empty,
            Body = body,
            IsBodyHtml = hasHtml
        };

        message.To.Add(new MailAddress(toEmail));

        // 3) SMTP client setup
        using var client = new SmtpClient(_options.Smtp.Host, _options.Smtp.Port)
        {
            EnableSsl = _options.Smtp.UseSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        // Username/password varsa istifadə et
        if (!string.IsNullOrWhiteSpace(_options.Smtp.UserName))
        {
            client.Credentials = new NetworkCredential(_options.Smtp.UserName, _options.Smtp.Password);
        }

        // 4) Send (SmtpClient cancellation token dəstəyi zəifdir, ona görə manual check edirik)
        ct.ThrowIfCancellationRequested();
        await client.SendMailAsync(message);
    }
}
