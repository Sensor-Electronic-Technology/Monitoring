using MassTransit.Mediator;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Services;
using MonitoringSystem.Shared.Services;

namespace MonitoringData.DataLoggingService; 

public class MonitorConfigDatabase:BackgroundService {

    private readonly IMediator _mediator;
    private readonly IMongoDatabase _database;
    private readonly IMonitorConfigurationProvider _configProvider;
    private readonly MonitorDataLogSettings _settings;
    private readonly ILogger<MonitorConfigDatabase> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public MonitorConfigDatabase(DataLogConfigProvider provider, IMediator mediator,
        ILogger<MonitorConfigDatabase> logger,IHostApplicationLifetime appLifetime) {
        this._applicationLifetime = appLifetime;
        this._configProvider = provider;
        this._mediator = mediator;
        this._logger = logger;
        this._settings = provider.MonitorDataLogSettings;
        var client = new MongoClient(this._settings.ConnectionString);
        this._database = client.GetDatabase(this._settings.DatabaseName);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        using var cursor = await this._database.WatchAsync(cancellationToken: stoppingToken);
        foreach (var change in cursor.ToEnumerable()) {
            /*this._logger.LogInformation("Change Detected: {S}", change.BackingDocument.ToString());
            this._applicationLifetime.StopApplication();*/
            await this._mediator.Publish<ReloadConsumer>(new ReloadConsumer(), stoppingToken);
        }
    }
}