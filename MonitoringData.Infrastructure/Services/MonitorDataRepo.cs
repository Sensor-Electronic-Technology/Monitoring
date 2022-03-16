using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Configuration;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services {
    public interface IMonitorDataService {

    }

    public class MonitorDataService {
        private IMongoClient _client;
        private IMongoDatabase _database;

        private MonitorDevice _monitorDevice;
        //private IMongoCollection<AnalogChannel> _analogChannels;
        //private IMongoCollection<DiscreteChannel> _discreteChannels;
        //private IMongoCollection<OutputItem> _outputChannels;
        //private IMongoCollection<VirtualChannel> _virtualChannels;
        //private IMongoCollection<MonitorAlert> _monitorAlerts;
        //private IMongoCollection<ActionItem> _monitorActions;

        private List<AnalogChannel> _analogChannels;
        private List<DiscreteChannel> _discreteChannels;
        private List<OutputItem> _outputChannels;
        private List<VirtualChannel> _virtualChannels;
        private List<MonitorAlert> _monitorAlerts;
        private List<ActionItem> _monitorActions;

        private IMongoCollection<AnalogReading> _analogReadings;
        private IMongoCollection<DiscreteReading> _discreteReadings;
        private IMongoCollection<OutputReading> _outputReadings;
        private IMongoCollection<VirtualReading> _virtualReadings;
        private IMongoCollection<ActionReading> _actionReadings;
        private IMongoCollection<AlertReading> _alertReadings;

        public MonitorDataService(IOptions<MonitorDatabaseSettings> databaseSettings) {
            this._client = new MongoClient(databaseSettings.Value.ConnectionString);
            this._database = this._client.GetDatabase(databaseSettings.Value.DatabaseName);
        }

        public MonitorDataService(string connectionString,string databaseName) {
            this._client = new MongoClient(connectionString);
            this._database = this._client.GetDatabase(databaseName);
        }

        private async Task InsertManyAsync(IEnumerable<AnalogReading> readings) {
            await this._analogReadings.InsertManyAsync(readings);
        }

        private async Task InsertManyAsync(IEnumerable<DiscreteReading> readings) {
            await this._discreteReadings.InsertManyAsync(readings);
        }

        private async Task InsertManyAsync(IEnumerable<OutputReading> readings) {
            await this._outputReadings.InsertManyAsync(readings);
        }

        private async Task InsertManyAsync(IEnumerable<VirtualReading> readings) {
            await this._virtualReadings.InsertManyAsync(readings);
        }

        private async Task InsertManyAsync(IEnumerable<ActionReading> readings) {
            await this._actionReadings.InsertManyAsync(readings);
        }

        private async Task InsertManyAsync(IEnumerable<AlertReading> readings) {
            await this._alertReadings.InsertManyAsync(readings);
        }

        private async Task LoadAsync() {
            this._analogChannels = await this._database.GetCollection<AnalogChannel>("analog_items").Find(_=>true).ToListAsync();
            this._discreteChannels = await this._database.GetCollection<DiscreteChannel>("discrete_items").Find(_ => true).ToListAsync();
            this._outputChannels = await this._database.GetCollection<OutputItem>("output_items").Find(_ => true).ToListAsync();
            this._virtualChannels = await this._database.GetCollection<VirtualChannel>("virtual_channel").Find(_ => true).ToListAsync();
            this._monitorActions = await this._database.GetCollection<ActionItem>("action_items").Find(_ => true).ToListAsync();
            this._monitorAlerts = await this._database.GetCollection<MonitorAlert>("alert_items").Find(_ => true).ToListAsync();

            this._analogReadings = this._database.GetCollection<AnalogReading>("analog_readings");
            this._discreteReadings = this._database.GetCollection<DiscreteReading>("dicrete_readings");
            this._virtualReadings = this._database.GetCollection<VirtualReading>("virtual_readings");
            this._outputReadings = this._database.GetCollection<OutputReading>("output_readings");
            this._actionReadings = this._database.GetCollection<ActionReading>("action_readings");
            this._alertReadings = this._database.GetCollection<AlertReading>("alert_reading");
        }
    }
}
