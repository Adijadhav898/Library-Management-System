using System.Threading.Tasks;

namespace LMS.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendHtmlAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}