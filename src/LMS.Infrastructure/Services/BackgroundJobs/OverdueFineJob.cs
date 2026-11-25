using Hangfire;
using LMS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Services.BackgroundJobs;

public class OverdueFineJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OverdueFineJob> _logger;

    public OverdueFineJob(IServiceProvider serviceProvider, ILogger<OverdueFineJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessOverdueFinesAsync()
    {
        _logger.LogInformation("Starting overdue fines processing job at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var circulationService = scope.ServiceProvider.GetRequiredService<ICirculationService>();

        try
        {
            await circulationService.ProcessOverdueFinesAsync();
            _logger.LogInformation("Overdue fines processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing overdue fines");
            throw;
        }
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessExpiredReservationsAsync()
    {
        _logger.LogInformation("Starting expired reservations processing job at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var circulationService = scope.ServiceProvider.GetRequiredService<ICirculationService>();

        try
        {
            await circulationService.ProcessExpiredReservationsAsync();
            _logger.LogInformation("Expired reservations processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired reservations");
            throw;
        }
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendDueDateRemindersAsync()
    {
        _logger.LogInformation("Starting due date reminders job at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            await notificationService.SendDueDateRemindersAsync();
            _logger.LogInformation("Due date reminders sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending due date reminders");
            throw;
        }
    }
}