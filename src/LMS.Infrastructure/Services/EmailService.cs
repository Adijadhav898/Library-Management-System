using LMS.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending using SMTP or a service like SendGrid, Mailgun, etc.
        // For now, this is a placeholder that logs the email
        await Task.CompletedTask;
        Console.WriteLine($"[EMAIL] To: {to}, Subject: {subject}, Body: {body}");
    }

    public async Task SendHtmlAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual HTML email sending
        await Task.CompletedTask;
        Console.WriteLine($"[EMAIL-HTML] To: {to}, Subject: {subject}, HTML Body: {htmlBody}");
    }
}

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}