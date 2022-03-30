using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.Infrastructure.Model;
using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Services.DataAccess {
    public interface IMonitorDataRepo {
        List<AnalogChannel> AnalogItems { get; }
        List<DiscreteChannel> DiscreteItems { get; }
        List<OutputItem> OutputItems { get; }
        List<VirtualChannel> VirtualItems { get; }
        List<MonitorAlert> MonitorAlerts { get; }
        List<ActionItem> ActionItems { get; }
        MonitorDevice MonitorDevice { get; }
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
    public class MonitorDataService : IMonitorDataRepo {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private readonly ILogger<MonitorDataService> _logger;
        private bool _initialized=false;

        public MonitorDevice MonitorDevice { get; private set; }
        public List<AnalogChannel> AnalogItems { get; private set; }
        public List<DiscreteChannel> DiscreteItems { get; private set; }
        public List<OutputItem> OutputItems { get; private set; }
        public List<VirtualChannel> VirtualItems { get; private set; }
        public List<MonitorAlert> MonitorAlerts { get; private set; }
        public List<ActionItem> ActionItems { get; private set; }

        private IMongoCollection<MonitorAlert> _monitorAlerts;
        private IMongoCollection<AnalogChannel> _analogItems;
        private IMongoCollection<DiscreteChannel> _discreteItems;
        private IMongoCollection<OutputItem> _outputItems;
        private IMongoCollection<VirtualChannel> _virtualItems;
        private IMongoCollection<ActionItem> _actionItems;
        private IMongoCollection<MonitorDevice> _deviceConfigurations;

        private IMongoCollection<AnalogReading> _analogReadings;
        private IMongoCollection<DiscreteReading> _discreteReadings;
        private IMongoCollection<OutputReading> _outputReadings;
        private IMongoCollection<VirtualReading> _virtualReadings;
        private IMongoCollection<ActionReading> _actionReadings;
        private IMongoCollection<AlertReading> _alertReadings;
        private IMongoCollection<DeviceReading> _deviceReadings;
        public MonitorDataService(IOptions<MonitorDatabaseSettings> databaseSettings,ILogger<MonitorDataService> logger) {
            this._client = new MongoClient(databaseSettings.Value.ConnectionString);
            this._database = this._client.GetDatabase(databaseSettings.Value.DatabaseName);
            this._logger = logger;
            this._initialized = false;
            
            this._analogReadings = this._database.GetCollection<AnalogReading>(databaseSettings.Value.AnalogReadingCollection);
            this._discreteReadings = this._database.GetCollection<DiscreteReading>(databaseSettings.Value.DiscreteReadingCollection);
            this._virtualReadings = this._database.GetCollection<VirtualReading>(databaseSettings.Value.VirtualReadingCollection);
            this._alertReadings = this._database.GetCollection<AlertReading>(databaseSettings.Value.AlertReadingCollection);
            this._actionReadings = this._database.GetCollection<ActionReading>(databaseSettings.Value.ActionReadingCollection);
            this._outputReadings = this._database.GetCollection<OutputReading>(databaseSettings.Value.OutputReadingCollection);
            this._deviceReadings = this._database.GetCollection<DeviceReading>(databaseSettings.Value.DeviceReadingCollection);

            this._actionItems = this._database.GetCollection<ActionItem>(databaseSettings.Value.ActionItemCollection);
            this._analogItems = this._database.GetCollection<AnalogChannel>(databaseSettings.Value.AnalogItemCollection);
            this._discreteItems = this._database.GetCollection<DiscreteChannel>(databaseSettings.Value.DiscreteItemCollection);
            this._virtualItems = this._database.GetCollection<VirtualChannel>(databaseSettings.Value.VirtualItemColleciton);
            this._outputItems = this._database.GetCollection<OutputItem>(databaseSettings.Value.OutputItemCollection);
            this._monitorAlerts = this._database.GetCollection<MonitorAlert>(databaseSettings.Value.AlertItemCollection);
            this._deviceConfigurations = this._database.GetCollection<MonitorDevice>(databaseSettings.Value.MonitorDeviceCollection);
        }
        public MonitorDataService(string connectionString, string databaseName,Dictionary<Type,string> collectionNames) {
            this._client = new MongoClient(connectionString);
            this._database = this._client.GetDatabase(databaseName);
            this._initialized = false;
            this._analogReadings = this._database.GetCollection<AnalogReading>(collectionNames[typeof(AnalogReading)]);
            this._discreteReadings = this._database.GetCollection<DiscreteReading>(collectionNames[typeof(DiscreteReading)]);
            this._virtualReadings = this._database.GetCollection<VirtualReading>(collectionNames[typeof(VirtualReading)]);
            this._outputReadings = this._database.GetCollection<OutputReading>(collectionNames[typeof(OutputReading)]);
            this._alertReadings = this._database.GetCollection<AlertReading>(collectionNames[typeof(AlertReading)]);
            this._actionReadings = this._database.GetCollection<ActionReading>(collectionNames[typeof(ActionReading)]);
            this._deviceReadings = this._database.GetCollection<DeviceReading>(collectionNames[typeof(DeviceReading)]);

            this._analogItems = this._database.GetCollection<AnalogChannel>(collectionNames[typeof(AnalogChannel)]);
            this._discreteItems = this._database.GetCollection<DiscreteChannel>(collectionNames[typeof(DiscreteChannel)]);
            this._virtualItems = this._database.GetCollection<VirtualChannel>(collectionNames[typeof(VirtualChannel)]);
            this._outputItems = this._database.GetCollection<OutputItem>(collectionNames[typeof(OutputItem)]);
            this._monitorAlerts = this._database.GetCollection<MonitorAlert>(collectionNames[typeof(MonitorAlert)]);
            this._actionItems = this._database.GetCollection<ActionItem>(collectionNames[typeof(ActionItem)]);
            this._deviceConfigurations = this._database.GetCollection<MonitorDevice>(collectionNames[typeof(MonitorDevice)]);
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
            this.AnalogItems = await (await this._analogItems.FindAsync(_ => true)).ToListAsync();
            this.DiscreteItems = await (await this._discreteItems.FindAsync(_ => true)).ToListAsync();
            this.OutputItems =await (await this._outputItems.FindAsync(_ => true)).ToListAsync();
            this.VirtualItems = await (await this._virtualItems.FindAsync(_ => true)).ToListAsync();
            this.ActionItems = await (await this._actionItems.FindAsync(_ => true)).ToListAsync();
            this.MonitorAlerts = await (await this._monitorAlerts.FindAsync(_ => true)).ToListAsync();
            var latest = this._deviceConfigurations.AsQueryable().Where(_ => true).Max(e =>(DateTime?)e.Created);
            if(latest is not null) {
                var monitorDevice = await this._deviceConfigurations.Find(e => e.Created == latest).FirstOrDefaultAsync();
                if(monitorDevice is not null) {
                    this.MonitorDevice = monitorDevice;
                    this._initialized = true;
                }
            }
        }
    }
}
