using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringSystem.Shared.Data;
using MonitoringWeb.ControlService.Data;
namespace MonitoringWeb.ControlService.Services;



public interface IMonitorDeviceService {
    IList<ManagedDevice> AvailableDevices { get; }
    Task Load();
}

public class MonitorDeviceService:IMonitorDeviceService {

    
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

    public IList<ManagedDevice> AvailableDevices { get; }
    public Task Load() {
        throw new NotImplementedException();
    }
}