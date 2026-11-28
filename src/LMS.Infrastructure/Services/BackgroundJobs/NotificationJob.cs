using Hangfire;
using LMS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Services.BackgroundJobs;

public class NotificationJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationJob> _logger;

    public NotificationJob(IServiceProvider serviceProvider, ILogger<NotificationJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendOverdueNotificationsAsync()
    {
        _logger.LogInformation("Starting overdue notifications job at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var circulationService = scope.ServiceProvider.GetRequiredService<ICirculationService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            var overdueLoans = await circulationService.GetOverdueLoansAsync();
            foreach (var loan in overdueLoans)
            {
                await notificationService.SendOverdueNotificationAsync(loan.Id);
            }

            _logger.LogInformation("Overdue notifications sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending overdue notifications");
            throw;
        }
    }
}
