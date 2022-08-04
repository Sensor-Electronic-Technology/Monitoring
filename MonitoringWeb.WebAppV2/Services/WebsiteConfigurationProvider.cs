using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.WebAppV2.Data;

namespace MonitoringWeb.WebAppV2.Services; 

public class WebsiteConfigurationProvider:IMonitorConfigurationProvider {
    private readonly IMongoCollection<ManagedDevice> _deviceCollection;
    private readonly IMongoCollection<SensorType> _sensorCollection;
    private List<ManagedDevice> _devices;
    private List<SensorType> _sensors;

    private Dictionary<string, Tuple<string, IEnumerable<SensorType>>> _deviceLookup =
        new Dictionary<string, Tuple<string, IEnumerable<SensorType>>>();
    private bool _loaded = false;

    public IEnumerable<ManagedDevice> Devices => this._devices.AsEnumerable();
    public IEnumerable<SensorType> Sensors => this._sensors.AsEnumerable();
    public IEnumerable<string> HubAddresses => this._devices.Select(e => e.HubAddress);
    public Dictionary<string,Tuple<string,IEnumerable<SensorType>>> DeviceLookup => this._deviceLookup;

    public WebsiteConfigurationProvider(IMongoClient client, IOptions<MonitorWebsiteSettings> settings) {
        var database = client.GetDatabase(settings.Value.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(settings.Value.ManagedDeviceCollection);
        this._sensorCollection = database.GetCollection<SensorType>(settings.Value.SensorTypeCollection);
    }

    public ManagedDevice GetDevice(string key) {
        return this._loaded ? this._devices.FirstOrDefault(e => e.DeviceName == key)! : null!;
    }

    public SensorType GetSensor(int key) {
        return this._loaded ? this._sensors.FirstOrDefault(e => e._id == key)! : null!;
    }

    public IEnumerable<RemoteAction> GetRemoteActions(string deviceName) {
        if (!this._loaded)
            return null;
        var device = this._devices.FirstOrDefault(e => e.DeviceName == deviceName);
        if (device == null)
            return null;
        return device.RemoteActions.AsEnumerable();
    }

    public async Task Load() {
        this._devices = await this._deviceCollection.Find(_ => true).ToListAsync();
        this._sensors = await this._sensorCollection.Find(_ => true).ToListAsync();
        foreach(var device in this._devices) {
            List<SensorType> sensorTypes = new List<SensorType>();
            foreach (var id in device.SensorTypes) {
                var sensorType=this._sensors.FirstOrDefault(e => e._id == id);
                if (sensorType != null) {
                    sensorTypes.Add(sensorType);
                }
            }
            this._deviceLookup.Add(device.DeviceName,
                new Tuple<string,IEnumerable<SensorType>>(device.DatabaseName,sensorTypes.AsEnumerable()));
        }
        this._loaded = true;
    }

    public Task Reload() {
        return Task.CompletedTask;
    }
}