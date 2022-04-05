using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Services.DatabaseManagement {
    public interface IDbManagement {
        Task CreateDatabase(string deviceName);
        Task Initialize();
    }

    public class ReadingsDbManagement {
        private IMongoDatabase _database;
        private readonly MonitorDatabaseSettings _settings;
        private ILogger<ReadingsDbManagement> _logger;

        public ReadingsDbManagement(IOptions<MonitorDatabaseSettings> options) {
            this._settings = options.Value;
        }

        public async Task Initialize() {
            var client = new MongoClient(this._settings.ConnectionString);
            var databases = client.ListDatabaseNames().ToList();
            if (databases.Contains(this._settings.DatabaseName)) {
                this._database = client.GetDatabase(this._settings.DatabaseName);
                var collections = this._database.ListCollectionNames().ToList();
                bool[] checks = new bool[14];

                if (!collections.Contains(this._settings.AnalogItemCollection)) {

                }

                if (!collections.Contains(this._settings.AnalogItemCollection)) {

                }

                if (!collections.Contains(this._settings.AnalogItemCollection)) {

                }

                if (!collections.Contains(this._settings.AnalogItemCollection)) {

                }
                collections.Contains(this._settings.DiscreteItemCollection);
                collections.Contains(this._settings.VirtualItemColleciton);
                collections.Contains(this._settings.AnalogReadingCollection);
                collections.Contains(this._settings.DiscreteReadingCollection);
                collections.Contains(this._settings.VirtualReadingCollection);
                collections.Contains(this._settings.MonitorDeviceCollection);




            }
        }

    }
}
