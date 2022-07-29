using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MonitoringSystem.Shared.Data;
using MonitoringWeb.ControlService.Data;
namespace MonitoringWeb.ControlService.Services;

public interface IMonitorDeviceService {
    IEnumerable<ManagedDevice> AvailableDevices { get; }
    Task Load();
}

public class MonitorDeviceService:IMonitorDeviceService {
    private List<ManagedDevice> _availableDevices=new List<ManagedDevice>();
    private IMongoCollection<ManagedDevice> _deviceCollection;
    private MonitorControlSettings _settings;
    public IEnumerable<ManagedDevice> AvailableDevices => this._availableDevices;
    
    public MonitorDeviceService(IOptions<MonitorControlSettings> options) {
        this._settings = options.Value;
        var client = new MongoClient(this._settings.ConnectionString);
        var database = client.GetDatabase(this._settings.DatabaseName);
        this._deviceCollection = database.GetCollection<ManagedDevice>(this._settings.ManagedDeviceCollection);
    }
    
    public async Task Load() {
        this._availableDevices= await this._deviceCollection.Find(e => e.DeviceType=="MonitoringBox").ToListAsync();
    }
}