using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringData.ModbusControlService.Data;
using MonitoringSystem.Shared.Data;
namespace MonitoringData.ModbusControlService.Services;

public class DeviceControlData {
    public MonitorDevice MonitorDevice { get; set; }
    public IEnumerable<VirtualChannel> VirtualChannels { get; set; }
}

public interface IMonitorDeviceService {
    Task<MonitorDevice> LoadDevice(string identifier);
}

public class MonitorDeviceService:IMonitorDeviceService {
    private IMongoCollection<MonitorDevice> _deviceCollection;
    private IMongoCollection<VirtualChannel> _channelCollection;

    private MonitorControlSettings _settings;
    
    private MonitorDeviceService(IOptions<MonitorControlSettings> options) {
    
    }

    public IEnumerable<VirtualChannel> VirtualChannels {
        get => this._virtualChannels.AsEnumerable();
    }

    private List<VirtualChannel> _virtualChannels;

    public async Task<MonitorDevice> LoadDevice(string identifier) {
        
    }

    public Task<IEnumerable<VirtualChannel>> GetVirtualChannels() {
        throw new NotImplementedException();
    }
}