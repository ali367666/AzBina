namespace Application.Abstracts.Services;

public interface IEmailSender
{
    Task SendAsync(
        string toemail,
        string subject,
        string htmlbody,
        string? textbody = null,
        CancellationToken ct = default);
}
