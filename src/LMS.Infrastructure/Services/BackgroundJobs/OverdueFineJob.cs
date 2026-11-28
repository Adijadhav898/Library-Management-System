using Hangfire;
using LMS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task ProcessOverdueLoansAsync()
    {
        _logger.LogInformation("Starting overdue loans processing job at {Time}", DateTime.UtcNow);

        using var scope = _serviceProvider.CreateScope();
        var circulationService = scope.ServiceProvider.GetRequiredService<ICirculationService>();

        try
        {
            await circulationService.ProcessOverdueLoansAsync();
            _logger.LogInformation("Overdue loans processing completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing overdue loans");
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
}
