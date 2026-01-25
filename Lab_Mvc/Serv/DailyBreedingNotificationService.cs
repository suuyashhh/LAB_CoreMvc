using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Lab_Mvc.Interfaces.DairyFarm;

public class DailyBreedingNotificationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyBreedingNotificationService> _logger;

    public DailyBreedingNotificationService(
        IServiceProvider serviceProvider,
        ILogger<DailyBreedingNotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Breeding Notification Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // ====== INDIA TIME ======
                var indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                var nowUtc = DateTime.UtcNow;
                var indiaNow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, indiaZone);

                // ====== Next Run = Tomorrow 12:00 AM IST ======
                var nextRun = indiaNow.Date.AddDays(1);

                var delay = nextRun - indiaNow;

                _logger.LogInformation("Next Breeding Job will run at IST: {Time}", nextRun);

                await Task.Delay(delay, stoppingToken);

                // ====== RUN JOB ======
                await RunDailyJob();
            }
            catch (TaskCanceledException)
            {
                // App shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Daily Breeding Notification Service loop");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task RunDailyJob()
    {
        _logger.LogInformation("Daily Breeding Notification Job Started");

        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<INotification>();

        var userIds = await repo.GetAllUserIds();

        _logger.LogInformation("Total Users Found: {Count}", userIds.Count);

        foreach (var userId in userIds)
        {
            try
            {
                _logger.LogInformation("Processing userId: {UserId}", userId);

                // 👉 तुमची existing method वापरतो
                await repo.RunBreedingUpdateOnly(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing userId {UserId}", userId);
            }
        }

        _logger.LogInformation("Daily Breeding Notification Job Completed");
    }
}
