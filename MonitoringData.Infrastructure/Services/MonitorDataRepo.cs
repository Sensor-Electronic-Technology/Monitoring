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
        List<AnalogChannel> AnalogItems { get; }
        List<DiscreteChannel> DiscreteItems { get; }
        List<OutputItem> OutputItems { get; }
        List<VirtualChannel> VirtualItems { get; }
        List<MonitorAlert> MonitorAlertCache { get; }
        List<ActionItem> ActionItems { get; }
        Task InsertManyAsync(IEnumerable<ActionReading> readings);
        Task InsertManyAsync(IEnumerable<AlertReading> readings);
        Task InsertManyAsync(IEnumerable<AnalogReading> readings);
        Task InsertManyAsync(IEnumerable<DiscreteReading> readings);
        Task InsertManyAsync(IEnumerable<OutputReading> readings);
        Task InsertManyAsync(IEnumerable<VirtualReading> readings);
        Task InsertDeviceReadingAsync(DeviceReading reading);
        Task<MonitorAlert> GetMonitorAlert(int alertId);
        Task UpdateAlert(int alertId, UpdateDefinition<MonitorAlert> update);
        Task LoadAsync();
    }

    public class MonitorDataService : IMonitorDataService {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private MonitorDevice _monitorDevice;
        public List<AnalogChannel> AnalogItems { get; private set; }
        public List<DiscreteChannel> DiscreteItems { get; private set; }
        public List<OutputItem> OutputItems { get; private set; }
        public List<VirtualChannel> VirtualItems { get; private set; }
        public List<MonitorAlert> MonitorAlertCache { get; private set; }
        public List<ActionItem> ActionItems { get; private set; }

        private IMongoCollection<MonitorAlert> _monitorAlerts;
        private IMongoCollection<AnalogReading> _analogReadings;
        private IMongoCollection<DiscreteReading> _discreteReadings;
        private IMongoCollection<OutputReading> _outputReadings;
        private IMongoCollection<VirtualReading> _virtualReadings;
        private IMongoCollection<ActionReading> _actionReadings;
        private IMongoCollection<AlertReading> _alertReadings;
        private IMongoCollection<DeviceReading> _deviceReadings;

        public MonitorDataService(IOptions<MonitorDatabaseSettings> databaseSettings) {
            this._client = new MongoClient(databaseSettings.Value.ConnectionString);
            this._database = this._client.GetDatabase(databaseSettings.Value.DatabaseName);
        }

        public MonitorDataService(string connectionString, string databaseName) {
            this._client = new MongoClient(connectionString);
            this._database = this._client.GetDatabase(databaseName);
        }

        public async Task InsertManyAsync(IEnumerable<AnalogReading> readings) {
            await this._analogReadings.InsertManyAsync(readings);
        }

        public async Task InsertManyAsync(IEnumerable<DiscreteReading> readings) {
            await this._discreteReadings.InsertManyAsync(readings);
        }

        public async Task InsertManyAsync(IEnumerable<OutputReading> readings) {
            await this._outputReadings.InsertManyAsync(readings);
        }

        public async Task InsertManyAsync(IEnumerable<VirtualReading> readings) {
            await this._virtualReadings.InsertManyAsync(readings);
        }

        public async Task InsertManyAsync(IEnumerable<ActionReading> readings) {
            await this._actionReadings.InsertManyAsync(readings);
        }

        public async Task InsertManyAsync(IEnumerable<AlertReading> readings) {
            await this._alertReadings.InsertManyAsync(readings);
        }

        public async Task InsertDeviceReadingAsync(DeviceReading reading) {
            await this._deviceReadings.InsertOneAsync(reading);
        }

        public async Task<MonitorAlert> GetMonitorAlert(int alertId) {
            return await this._monitorAlerts.Find(e => e._id == alertId).FirstOrDefaultAsync();
        }

        public async Task UpdateAlert(int alertId,UpdateDefinition<MonitorAlert> update) {
            await this._monitorAlerts.UpdateOneAsync(e => e._id == alertId, update);
        }



        public async Task LoadAsync() {
            this.AnalogItems = await this._database.GetCollection<AnalogChannel>("analog_items").Find(_ => true).ToListAsync();
            this.DiscreteItems = await this._database.GetCollection<DiscreteChannel>("discrete_items").Find(_ => true).ToListAsync();
            this.OutputItems = await this._database.GetCollection<OutputItem>("output_items").Find(_ => true).ToListAsync();
            this.VirtualItems = await this._database.GetCollection<VirtualChannel>("virtual_items").Find(_ => true).ToListAsync();
            this.ActionItems = await this._database.GetCollection<ActionItem>("action_items").Find(_ => true).ToListAsync();
            this.MonitorAlertCache = await this._database.GetCollection<MonitorAlert>("alert_items").Find(_ => true).ToListAsync();

            this._analogReadings = this._database.GetCollection<AnalogReading>("analog_readings");
            this._discreteReadings = this._database.GetCollection<DiscreteReading>("discrete_readings");
            this._virtualReadings = this._database.GetCollection<VirtualReading>("virtual_readings");
            this._outputReadings = this._database.GetCollection<OutputReading>("output_readings");
            this._actionReadings = this._database.GetCollection<ActionReading>("action_readings");
            this._alertReadings = this._database.GetCollection<AlertReading>("alert_readings");
            this._deviceReadings = this._database.GetCollection<DeviceReading>("device_readings");
        }
    }
}
