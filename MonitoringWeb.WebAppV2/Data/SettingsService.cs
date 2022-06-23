using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
namespace MonitoringWeb.WebAppV2.Data;

public class SettingsService {
    private MonitorWebsiteSettings _settings;
    private IMongoCollection<ManagedDevice> _deviceCollection;
    private IMongoCollection<SensorType> _sensorCollection;
    private List<ManagedDevice> _devices;
    private List<SensorType> _sensors;

    public SettingsService(IOptions<MonitorWebsiteSettings> options,IMongoClient client) {
        this._settings = options.Value;
        var database = client.GetDatabase(this._settings.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(this._settings.ManagedDeviceCollection);
        this._sensorCollection = database.GetCollection<SensorType>(this._settings.SensorTypeCollection);
    }

    public Task<IEnumerable<ManagedDevice>> GetDevices() {
        return Task.FromResult(this._devices.AsEnumerable());
    }

    public Task<IEnumerable<SensorType>> GetSensors() {
        return Task.FromResult(this._sensors.AsEnumerable());
    }

    public Task<IEnumerable<string>> GetHubAddresses() {
        return Task.FromResult(this._devices.Select(e => e.HubAddress));
    }

    public async Task Load() {
        this._devices=await this._deviceCollection.Find(_ => true).ToListAsync();
        this._sensors = await this._sensorCollection.Find(_ => true).ToListAsync();
    }
}

