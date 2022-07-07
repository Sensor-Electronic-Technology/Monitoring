using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MonitoringSystem.Shared.Data;
using MonitoringWeb.ControlService.Data;
namespace MonitoringWeb.ControlService.Services;
public class DeviceControlData {
    private IMongoCollection<VirtualChannel> _virtualCollection;
    private IMongoCollection<MonitorDevice> _deviceCollection;
    public ManagedDevice Device { get; set; }
    public MonitorDevice MonitorDevice { get; set; }
    public List<VirtualChannel> VirtualChannels { get; set; }
    
    public DeviceControlData(IMongoClient mongoClient,ManagedDevice device) {
        var database = mongoClient.GetDatabase(device.DatabaseName);
        this._virtualCollection = database.GetCollection<VirtualChannel>("virtual_items");
        this._deviceCollection = database.GetCollection<MonitorDevice>("device_items");
        this.Device = device;
    }
    public async Task Load() {
        this.VirtualChannels = await this._virtualCollection.Find(_ => true).ToListAsync();
        this.MonitorDevice = await this._deviceCollection.Find(_ => true).FirstAsync();
    }
}

public interface IMonitorDeviceService {
    IEnumerable<DeviceControlData> AvailableDevices { get; }
    Task Load();
}

public class MonitorDeviceService:IMonitorDeviceService {
    private List<DeviceControlData> _availableDevices=new List<DeviceControlData>();
    private IMongoCollection<ManagedDevice> _deviceCollection;
    private MonitorControlSettings _settings;
    public IEnumerable<DeviceControlData> AvailableDevices => this._availableDevices;
    
    public MonitorDeviceService(IOptions<MonitorControlSettings> options) {
        this._settings = options.Value;
        var client = new MongoClient(this._settings.ConnectionString);
        var database = client.GetDatabase(this._settings.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(this._settings.ManagedDeviceCollection);
    }
    
    public async Task Load() {
        var devices = await this._deviceCollection.Find(e => e.DeviceType=="MonitoringBox").ToListAsync();
        foreach (var device in devices) {
            var deviceData = new DeviceControlData(this._deviceCollection.Database.Client, device);
            await deviceData.Load();
            this._availableDevices.Add(deviceData);
        }
    }
}