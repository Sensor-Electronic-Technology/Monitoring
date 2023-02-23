using MassTransit.Mediator;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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
            this._applicationLifetime.StopApplication();
            /*var collectionName = change.CollectionNamespace.CollectionName;
            this._logger.LogCritical(collectionName);
            if (collectionName == "website_images.files") {
                await this.DownloadImage();
            } else {
                await this._mediator.Publish<ReloadConsumer>(new ReloadConsumer(), stoppingToken);
            }*/
        }
    }

    private async Task DownloadImage() {
        this._logger.LogInformation("Starting download..");
        var bucket = new GridFSBucket(this._database, new GridFSBucketOptions {
            BucketName = "website_images"
        });
        
        var options = new GridFSDownloadByNameOptions { Revision = -1 };
                
        using (var stream = new FileStream(@"GasDetectorMap_New.png", FileMode.Append, FileAccess.Write)) {
            await bucket.DownloadToStreamByNameAsync("GasDetectorMap.png", stream);
        }
                
        if (File.Exists("GasDetectorMap_New.png")) {
            if (File.Exists("GasDetectorMap.png")) {
                File.Delete("GasDetectorMap.png");
            }
            File.Copy("GasDetectorMap_New.png", "GasDetectorMap.png");
            File.Delete("GasDetectorMap_New.png");
            this._logger.LogInformation("New floor plan downloaded");
        } else {
            if (!File.Exists("GasDetectorMap.png")) {
                this._logger.LogCritical("Download failed and no existing floor plan found");
            } else {
                this._logger.LogWarning("GasDetectorMap.png not replaced.  Download Failed");
            }
        }
    }
}