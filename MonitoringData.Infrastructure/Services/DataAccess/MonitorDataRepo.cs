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
        List<TankScale> TankScales { get; }
        ManagedDevice ManagedDevice { get; }

        Task InsertOneAsync(AlertReadings readings);
        Task InsertOneAsync(AnalogReadings? readings);
        Task InsertOneAsync(DiscreteReadings? readings);
        Task InsertOneAsync(VirtualReadings? readings);
        Task InsertWeightReading(WeightReading weightReading);
        Task<AnalogReadings?> GetLastAnalogReading();
        Task LoadAsync();
        Task ReloadAsync();
    }
    public class MonitorDataService : IMonitorDataRepo {
        private readonly ILogger<MonitorDataService> _logger;
        private readonly DataLogConfigProvider _configProvider;
        private readonly IMongoClient _client;
        private ManagedDevice _device;

        public ManagedDevice ManagedDevice => this._device;
        public List<AnalogItem> AnalogItems { get; private set; } = new List<AnalogItem>();
        public List<DiscreteItem> DiscreteItems { get; private set; } = new List<DiscreteItem>();
        public List<OutputItem> OutputItems { get; private set; } = new List<OutputItem>();
        public List<VirtualItem> VirtualItems { get; private set; } = new List<VirtualItem>();
        public List<MonitorAlert> MonitorAlerts { get; private set; } = new List<MonitorAlert>();
        public List<ActionItem> ActionItems { get; private set; } = new List<ActionItem>();
        public List<TankScale> TankScales { get; private set; } = new List<TankScale>();

        private IMongoCollection<MonitorAlert> _monitorAlerts;
        private IMongoCollection<AnalogItem> _analogItems;
        private IMongoCollection<DiscreteItem> _discreteItems;
        private IMongoCollection<OutputItem> _outputItems;
        private IMongoCollection<VirtualItem> _virtualItems;
        private IMongoCollection<ActionItem> _actionItems;
        private IMongoCollection<TankScale> _tankScales;

        private IMongoCollection<AnalogReadings?> _analogReadings;
        private IMongoCollection<DiscreteReadings?> _discreteReadings;
        private IMongoCollection<VirtualReadings?> _virtualReadings;
        private IMongoCollection<AlertReadings> _alertReadings;
        private IMongoCollection<WeightReading> _weightReadings;

        public MonitorDataService(IMongoClient client,DataLogConfigProvider configProvider,
            ILogger<MonitorDataService> logger) {
            this._client = client;
            this._configProvider = configProvider;
            this._device = this._configProvider.ManagedDevice;
            this._logger = logger;
            var database = this._client.GetDatabase(this._device.DatabaseName);
            this._analogReadings = database.GetCollection<AnalogReadings?>(this._device.CollectionNames[nameof(AnalogReadings)]);
            this._discreteReadings = database.GetCollection<DiscreteReadings?>(this._device.CollectionNames[nameof(DiscreteReadings)]);
            this._virtualReadings = database.GetCollection<VirtualReadings?>(this._device.CollectionNames[nameof(VirtualReadings)]);
            this._alertReadings = database.GetCollection<AlertReadings>(this._device.CollectionNames[nameof(AlertReadings)]);
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._analogItems = database.GetCollection<AnalogItem>(this._device.CollectionNames[nameof(AnalogItem)]);
            this._discreteItems = database.GetCollection<DiscreteItem>(this._device.CollectionNames[nameof(DiscreteItem)]);
            this._virtualItems = database.GetCollection<VirtualItem>(this._device.CollectionNames[nameof(VirtualItem)]);
            this._outputItems = database.GetCollection<OutputItem>(this._device.CollectionNames[nameof(OutputItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
            var tankScaleDatabase = this._client.GetDatabase("nh3_logs");
            this._tankScales = tankScaleDatabase.GetCollection<TankScale>("tank_scales");
            this._weightReadings = tankScaleDatabase.GetCollection<WeightReading>("weight_readings");
            
        }
        
        public async Task InsertOneAsync(AlertReadings readings) {
            await this._alertReadings.InsertOneAsync(readings);
        }

        public async Task InsertOneAsync(AnalogReadings? readings) {
            await this._analogReadings.InsertOneAsync(readings);
        }

        public async Task InsertOneAsync(DiscreteReadings? readings) {
            await this._discreteReadings.InsertOneAsync(readings);
        }

        public async Task InsertOneAsync(VirtualReadings? readings) {
            await this._virtualReadings.InsertOneAsync(readings);
        }
        public async Task InsertWeightReading(WeightReading weightReading) {
            await this._weightReadings.InsertOneAsync(weightReading);
        }

        public async Task<AnalogReadings?> GetLastAnalogReading() {
            SortDefinition<AnalogReadings?> sort="{ timestamp: -1 }";
            return await this._analogReadings.Find(_=>true).Sort(sort).FirstOrDefaultAsync();
        }

        public async Task LoadAsync() {
            this.AnalogItems = await this._analogItems.Find(_ => true).ToListAsync();
            this.DiscreteItems = await this._discreteItems.Find(_ => true).ToListAsync();
            this.OutputItems = await this._outputItems.Find(_ => true).ToListAsync();
            this.VirtualItems = await this._virtualItems.Find(_ => true).ToListAsync();
            this.ActionItems = await this._actionItems.Find(_ => true).ToListAsync();
            this.MonitorAlerts = await this._monitorAlerts.Find(_ => true).ToListAsync();
            this.TankScales = await this._tankScales.Find(_ => true).ToListAsync();
        }

        public async Task ReloadAsync() {
            this._logger.LogInformation("MonitorDataRepo Reloading");
            await this._configProvider.Reload();
            this._device = this._configProvider.ManagedDevice;
            var database = this._client.GetDatabase(this._device.DatabaseName);
            this._analogReadings = database.GetCollection<AnalogReadings?>(this._device.CollectionNames[nameof(AnalogReadings)]);
            this._discreteReadings = database.GetCollection<DiscreteReadings?>(this._device.CollectionNames[nameof(DiscreteReadings)]);
            this._virtualReadings = database.GetCollection<VirtualReadings?>(this._device.CollectionNames[nameof(VirtualReadings)]);
            this._alertReadings = database.GetCollection<AlertReadings>(this._device.CollectionNames[nameof(AlertReadings)]);
            this._actionItems = database.GetCollection<ActionItem>(this._device.CollectionNames[nameof(ActionItem)]);
            this._analogItems = database.GetCollection<AnalogItem>(this._device.CollectionNames[nameof(AnalogItem)]);
            this._discreteItems = database.GetCollection<DiscreteItem>(this._device.CollectionNames[nameof(DiscreteItem)]);
            this._virtualItems = database.GetCollection<VirtualItem>(this._device.CollectionNames[nameof(VirtualItem)]);
            this._outputItems = database.GetCollection<OutputItem>(this._device.CollectionNames[nameof(OutputItem)]);
            this._monitorAlerts = database.GetCollection<MonitorAlert>(this._device.CollectionNames[nameof(MonitorAlert)]);
            var tankScaleDatabase = this._client.GetDatabase("nh3_logs");
            this._tankScales = tankScaleDatabase.GetCollection<TankScale>("tank_scales");
            this._weightReadings = tankScaleDatabase.GetCollection<WeightReading>("weight_readings");
            this.AnalogItems = await this._analogItems.Find(_ => true).ToListAsync();
            this.DiscreteItems = await this._discreteItems.Find(_ => true).ToListAsync();
            this.OutputItems = await this._outputItems.Find(_ => true).ToListAsync();
            this.VirtualItems = await this._virtualItems.Find(_ => true).ToListAsync();
            this.ActionItems = await this._actionItems.Find(_ => true).ToListAsync();
            this.MonitorAlerts = await this._monitorAlerts.Find(_ => true).ToListAsync();
            this.TankScales = await this._tankScales.Find(_ => true).ToListAsync();
        }
    }
}
