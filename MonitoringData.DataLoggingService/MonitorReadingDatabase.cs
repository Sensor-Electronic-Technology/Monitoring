using MassTransit.Mediator;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Services;

namespace MonitoringData.DataLoggingService {
    public class MonitorReadingDatabase : BackgroundService {
        private readonly IMediator _mediator;
        private readonly IMongoDatabase _database;
        private readonly MonitorDataLogSettings _settings;
        private readonly ILogger<MonitorReadingDatabase> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public MonitorReadingDatabase(IMediator mediator,DataLogConfigProvider configProvider,
            ILogger<MonitorReadingDatabase> logger,IHostApplicationLifetime appLifetime) {
            this._applicationLifetime = appLifetime;
            this._mediator = mediator;
            this._settings = configProvider.MonitorDataLogSettings;
            this._logger = logger;
            var client = new MongoClient(this._settings.ConnectionString);
            this._database = client.GetDatabase(configProvider.ManagedDevice.DatabaseName);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            using var cursor = await this._database.WatchAsync(cancellationToken: stoppingToken);
            foreach (var change in cursor.ToEnumerable()) {
                await this._mediator.Publish<ReloadConsumer>(new ReloadConsumer(), stoppingToken);
            }
        }
    }
}
