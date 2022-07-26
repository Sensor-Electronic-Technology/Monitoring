using MassTransit;
using MassTransit.Configuration;
using MassTransit.Mediator;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Events;
using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitoringData.Infrastructure.Data;
using MonitoringData.Infrastructure.Services;

namespace MonitoringData.DataLoggingService {
    public class MonitorDBChanges : BackgroundService {
        private readonly IMediator _mediator;
        private readonly IMongoDatabase _database;
        private readonly MonitorDataLogSettings _settings;

        public MonitorDBChanges(IMediator mediator,IOptions<MonitorDataLogSettings> options,DataLogConfigProvider configProvider) {
            this._mediator = mediator;
            this._settings = options.Value;
            var client = new MongoClient(options.Value.ConnectionString);
            this._database = client.GetDatabase(configProvider.ManagedDevice.DatabaseName);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            /*var client = new MongoClient(this._settings.ConnectionString);
            var database = client.GetDatabase(this._settings.DatabaseName);*/
            using (var cursor = await this._database.WatchAsync()) {
                foreach (var change in cursor.ToEnumerable()) {
                    Console.WriteLine(change.ToString());
                    await this._mediator.Publish<ReloadConsumer>(new ReloadConsumer());
                }
            }
        }
    }
}
