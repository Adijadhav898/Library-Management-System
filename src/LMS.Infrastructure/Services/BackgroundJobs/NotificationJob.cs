using Hangfire;
using LMS.Application.Interfaces;
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
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            await notificationService.SendOverdueNotificationsAsync();
            _logger.LogInformation("Overdue notifications sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending overdue notifications");
            throw;
        }
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendReservationReadyNotificationsAsync()
    {
        _logger.LogInformation("Starting reservation ready notifications job at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            await notificationService.SendReservationReadyNotificationsAsync();
            _logger.LogInformation("Reservation ready notifications sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending reservation ready notifications");
            throw;
        }
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendFineNotificationsAsync()
    {
        _logger.LogInformation("Starting fine notifications job at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            await notificationService.SendFineNotificationsAsync();
            _logger.LogInformation("Fine notifications sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending fine notifications");
            throw;
        }
    }
}