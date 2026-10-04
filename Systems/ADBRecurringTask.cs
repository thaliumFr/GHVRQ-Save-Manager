using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ADBRecurringTask: BackgroundService
{
    private Timer _timer;
    private readonly ILogger<ADBRecurringTask> _logger;

    public ADBRecurringTask(ILogger<ADBRecurringTask> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Background Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Timed Hosted Service is working.");
            ADB.CheckForDevices();
            Task.Delay(TimeSpan.FromSeconds(10), stoppingToken).Wait();
        }

        _logger.LogInformation("Timed Background Service is stopping.");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

}
