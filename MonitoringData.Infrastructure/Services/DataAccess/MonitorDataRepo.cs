using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringData.Infrastructure.Services.DataAccess {
    public interface IMonitorDataRepo {
        List<AnalogItem> AnalogItems { get; }
        List<DiscreteItem> DiscreteItems { get; }
        List<OutputItem> OutputItems { get; }
        List<VirtualItem> VirtualItems { get; }
        List<MonitorAlert> MonitorAlerts { get; }
        List<ActionItem> ActionItems { get; }
        ManagedDevice ManagedDevice { get; }

        Task InsertOneAsync(AlertReadings readings);
        Task InsertOneAsync(AnalogReadings readings);
        Task InsertOneAsync(DiscreteReadings readings);
        Task InsertOneAsync(VirtualReadings readings);
        Task<AnalogReadings> GetLastAnalogReading();
        Task LoadAsync();
        Task ReloadAsync();
    }
    public class MonitorDataService : IMonitorDataRepo {
        private readonly ILogger<MonitorDataService> _logger;
        private readonly DataLogConfigProvider _configProvider;
        private readonly IMongoClient _client;
        private ManagedDevice _device;

        public ManagedDevice ManagedDevice { get=>this._device;}
        public List<AnalogItem> AnalogItems { get; private set; }
        public List<DiscreteItem> DiscreteItems { get; private set; }
        public List<OutputItem> OutputItems { get; private set; }
        public List<VirtualItem> VirtualItems { get; private set; }
        public List<MonitorAlert> MonitorAlerts { get; private set; }
        public List<ActionItem> ActionItems { get; private set; }

        private IMongoCollection<MonitorAlert> _monitorAlerts;
        private IMongoCollection<AnalogItem> _analogItems;
        private IMongoCollection<DiscreteItem> _discreteItems;
        private IMongoCollection<OutputItem> _outputItems;
        private IMongoCollection<VirtualItem> _virtualItems;
        private IMongoCollection<ActionItem> _actionItems;

        private IMongoCollection<AnalogReadings> _analogReadings;
        private IMongoCollection<DiscreteReadings> _discreteReadings;
        private IMongoCollection<VirtualReadings> _virtualReadings;
        private IMongoCollection<AlertReadings> _alertReadings;

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
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._analogItems = database.GetCollection<AnalogItem>(this._device.CollectionNames[nameof(AnalogItem)]);
            this._discreteItems = database.GetCollection<DiscreteItem>(this._device.CollectionNames[nameof(DiscreteItem)]);
            this._virtualItems = database.GetCollection<VirtualItem>(this._device.CollectionNames[nameof(VirtualItem)]);
            this._outputItems = database.GetCollection<OutputItem>(this._device.CollectionNames[nameof(OutputItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
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

        public async Task<AnalogReadings> GetLastAnalogReading() {
            SortDefinition<AnalogReadings> sort="{ timestamp: -1 }";
            return await this._analogReadings.Find(_=>true).Sort(sort).FirstOrDefaultAsync();
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
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._analogItems = database.GetCollection<AnalogItem>(this._device.CollectionNames[nameof(AnalogItem)]);
            this._discreteItems = database.GetCollection<DiscreteItem>(this._device.CollectionNames[nameof(DiscreteItem)]);
            this._virtualItems = database.GetCollection<VirtualItem>(this._device.CollectionNames[nameof(VirtualItem)]);
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
