using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

        Task InsertOneAsync(AlertReadings readings);
        Task InsertOneAsync(AnalogReadings readings);
        Task InsertOneAsync(DiscreteReadings readings);
        Task InsertOneAsync(VirtualReadings readings);
        Task InsertDeviceReadingAsync(DeviceReading reading);
        Task UpdateAlert(int alertId, UpdateDefinition<MonitorAlert> update);
        Task LoadAsync();
    }
    public class MonitorDataService : IMonitorDataRepo {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private readonly ILogger<MonitorDataService> _logger;

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

        private IMongoCollection<AnalogReadings> _analogReadings;
        private IMongoCollection<DiscreteReadings> _discreteReadings;
        private IMongoCollection<VirtualReadings> _virtualReadings;
        private IMongoCollection<AlertReadings> _alertReadings;
        private IMongoCollection<DeviceReading> _deviceReadings;

        public MonitorDataService(IOptions<MonitorDatabaseSettings> databaseSettings,ILogger<MonitorDataService> logger) {
            this._client = new MongoClient(databaseSettings.Value.ConnectionString);
            this._database = this._client.GetDatabase(databaseSettings.Value.DatabaseName);
            this._logger = logger;
            
            this._analogReadings = this._database.GetCollection<AnalogReadings>(databaseSettings.Value.AnalogReadingCollection);
            this._discreteReadings = this._database.GetCollection<DiscreteReadings>(databaseSettings.Value.DiscreteReadingCollection);
            this._virtualReadings = this._database.GetCollection<VirtualReadings>(databaseSettings.Value.VirtualReadingCollection);
            this._alertReadings = this._database.GetCollection<AlertReadings>(databaseSettings.Value.AlertReadingCollection);

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
            this._analogReadings = this._database.GetCollection<AnalogReadings>(collectionNames[typeof(AnalogReading)]);
            this._discreteReadings = this._database.GetCollection<DiscreteReadings>(collectionNames[typeof(DiscreteReading)]);
            this._virtualReadings = this._database.GetCollection<VirtualReadings>(collectionNames[typeof(VirtualReading)]);
            //this._outputReadings = this._database.GetCollection<OutputReading>(collectionNames[typeof(OutputReading)]);
            this._alertReadings = this._database.GetCollection<AlertReadings>(collectionNames[typeof(AlertReading)]);
            //this._actionReadings = this._database.GetCollection<ActionReading>(collectionNames[typeof(ActionReading)]);
            this._deviceReadings = this._database.GetCollection<DeviceReading>(collectionNames[typeof(DeviceReading)]);

            this._analogItems = this._database.GetCollection<AnalogChannel>(collectionNames[typeof(AnalogChannel)]);
            this._discreteItems = this._database.GetCollection<DiscreteChannel>(collectionNames[typeof(DiscreteChannel)]);
            this._virtualItems = this._database.GetCollection<VirtualChannel>(collectionNames[typeof(VirtualChannel)]);
            this._outputItems = this._database.GetCollection<OutputItem>(collectionNames[typeof(OutputItem)]);
            this._monitorAlerts = this._database.GetCollection<MonitorAlert>(collectionNames[typeof(MonitorAlert)]);
            this._actionItems = this._database.GetCollection<ActionItem>(collectionNames[typeof(ActionItem)]);
            this._deviceConfigurations = this._database.GetCollection<MonitorDevice>(collectionNames[typeof(MonitorDevice)]);
        }

        public async Task InsertOneAsync(AlertReadings readings) {
            await this._alertReadings.InsertOneAsync(readings);
        }

        public async Task InsertOneAsync(AnalogReadings readings) {
            await this._analogReadings.InsertOneAsync(readings);
        }

        public async Task InsertOneAsync(DiscreteReadings readings) {
            await this._discreteReadings.InsertOneAsync(readings);
        }

        public async Task InsertOneAsync(VirtualReadings readings) {
            await this._virtualReadings.InsertOneAsync(readings);
        }

        public async Task InsertDeviceReadingAsync(DeviceReading reading) {
            await this._deviceReadings.InsertOneAsync(reading);
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
                }
            }
        }
    }
}
