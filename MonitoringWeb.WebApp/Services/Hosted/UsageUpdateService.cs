using MonitoringSystem.Shared.Services;

namespace MonitoringWeb.WebApp.Services.Hosted;

public class UsageUpdateService : BackgroundService {
    private readonly ILogger<UsageUpdateService> _logger;
    private readonly UsageService _usageService;
    private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(30);

    public UsageUpdateService(ILogger<UsageUpdateService> logger, UsageService usageService) {
        this._logger = logger;
        this._usageService = usageService;
    }

    public override Task StartAsync(CancellationToken cancellationToken) {
        this._logger.LogInformation("Usage Update Service started at {StartTime}", DateTime.Now);
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using var timer = new PeriodicTimer(this._updateInterval);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken)) {
            if (DateTime.Now.Hour == 0) 
                await this._usageService.UpdateAllUsageTables();
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken) {
        this._logger.LogInformation("Usage Update Service stopped at {EndTime}", DateTime.Now);
        return base.StopAsync(cancellationToken);
    }
}