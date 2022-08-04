using Microsoft.Extensions.Logging;
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
        ManagedDevice ManagedDevice { get; }

        Task InsertOneAsync(AlertReadings readings);
        Task InsertOneAsync(AnalogReadings readings);
        Task InsertOneAsync(DiscreteReadings readings);
        Task InsertOneAsync(VirtualReadings readings);
        Task InsertDeviceReadingAsync(DeviceReading reading);
        Task UpdateAlert(int alertId, UpdateDefinition<MonitorAlert> update);
        Task LoadAsync();
        Task ReloadAsync();
    }
    public class MonitorDataService : IMonitorDataRepo {
        private readonly ILogger<MonitorDataService> _logger;
        private readonly DataLogConfigProvider _configProvider;
        private readonly IMongoClient _client;
        private ManagedDevice _device;

        public ManagedDevice ManagedDevice { get=>this._device;}
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

        public MonitorDataService(IMongoClient client,DataLogConfigProvider configProvider,ILogger<MonitorDataService> logger) {
            this._client = client;
            this._configProvider = configProvider;
            this._device = this._configProvider.ManagedDevice;
            this._logger = logger;
            var database = this._client.GetDatabase(this._device.DatabaseName);
            this._analogReadings = database.GetCollection<AnalogReadings>(this._device.CollectionNames[nameof(AnalogReadings)]);
            this._discreteReadings = database.GetCollection<DiscreteReadings>(this._device.CollectionNames[nameof(DiscreteReadings)]);
            this._virtualReadings = database.GetCollection<VirtualReadings>(this._device.CollectionNames[nameof(VirtualReadings)]);
            this._alertReadings = database.GetCollection<AlertReadings>(this._device.CollectionNames[nameof(AlertReadings)]);
            this._deviceReadings = database.GetCollection<DeviceReading>(this._device.CollectionNames[nameof(DeviceReading)]);
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._analogItems = database.GetCollection<AnalogChannel>(this._device.CollectionNames[nameof(AnalogChannel)]);
            this._discreteItems = database.GetCollection<DiscreteChannel>(this._device.CollectionNames[nameof(DiscreteChannel)]);
            this._virtualItems = database.GetCollection<VirtualChannel>(this._device.CollectionNames[nameof(VirtualChannel)]);
            this._outputItems = database.GetCollection<OutputItem>(this._device.CollectionNames[nameof(OutputItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
            //this._deviceConfigurations = database.GetCollection<MonitorDevice>(this._device.CollectionNames[nameof(MonitorDevice)]);
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
        }

        public async Task ReloadAsync() {
            await this._configProvider.Reload();
            this._device = this._configProvider.ManagedDevice;
            var database = this._client.GetDatabase(this._device.DatabaseName);
            this._analogReadings = database.GetCollection<AnalogReadings>(this._device.CollectionNames[nameof(AnalogReadings)]);
            this._discreteReadings = database.GetCollection<DiscreteReadings>(this._device.CollectionNames[nameof(DiscreteReadings)]);
            this._virtualReadings = database.GetCollection<VirtualReadings>(this._device.CollectionNames[nameof(VirtualReadings)]);
            this._alertReadings = database.GetCollection<AlertReadings>(this._device.CollectionNames[nameof(AlertReadings)]);
            this._deviceReadings = database.GetCollection<DeviceReading>(this._device.CollectionNames[nameof(DeviceReading)]);
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._analogItems = database.GetCollection<AnalogChannel>(this._device.CollectionNames[nameof(AnalogChannel)]);
            this._discreteItems = database.GetCollection<DiscreteChannel>(this._device.CollectionNames[nameof(DiscreteChannel)]);
            this._virtualItems = database.GetCollection<VirtualChannel>(this._device.CollectionNames[nameof(VirtualChannel)]);
            this._outputItems = database.GetCollection<OutputItem>(this._device.CollectionNames[nameof(OutputItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
            this.AnalogItems = await (await this._analogItems.FindAsync(_ => true)).ToListAsync();
            this.DiscreteItems = await (await this._discreteItems.FindAsync(_ => true)).ToListAsync();
            this.OutputItems =await (await this._outputItems.FindAsync(_ => true)).ToListAsync();
            this.VirtualItems = await (await this._virtualItems.FindAsync(_ => true)).ToListAsync();
            this.ActionItems = await (await this._actionItems.FindAsync(_ => true)).ToListAsync();
            this.MonitorAlerts = await (await this._monitorAlerts.FindAsync(_ => true)).ToListAsync();
        }
    }
}
