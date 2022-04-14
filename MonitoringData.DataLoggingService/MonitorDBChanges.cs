using MassTransit;
using MassTransit.Mediator;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Events;
using MonitoringData.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.DataLoggingService {
    public class MonitorDBChanges : BackgroundService {
        private readonly IMediator _mediator;

        public MonitorDBChanges(IMediator mediator) {
            this._mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var client = new MongoClient("mongodb://172.20.3.41");
            var database = client.GetDatabase("epi1_data");
            IMongoCollection<AnalogChannel> analogItems=database.GetCollection<AnalogChannel>("analog_items");
            using(var cursor=await analogItems.WatchAsync()) {
                foreach(var change in cursor.ToEnumerable()) {
                    Console.WriteLine(change.ToString());
                    await this._mediator.Publish<ReloadConsumer>(new ReloadConsumer());
                }
            }
        }
    }
}
