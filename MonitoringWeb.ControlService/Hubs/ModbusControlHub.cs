using Microsoft.AspNetCore.SignalR;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.ControlService.Services;

namespace MonitoringWeb.ControlService.Hubs; 

public class MonitorControlHub : Hub<IModbusControlHub> {
    private readonly IModbusService _modbusService;
    private readonly IMonitorDeviceService _deviceService;
    
    public MonitorControlHub(IModbusService modbusService,IMonitorDeviceService deviceService) {
        this._modbusService = modbusService;
        this._deviceService = deviceService;
    }

    public async Task StartAsync() {
        await this._deviceService.Load();
    }

    public async Task ToggleMaintenance(string device) {
        //
    }

    public async Task ToggleRemoteAlarm(string device) {
        
    }

    public async Task ToggleRemoteWarning(string device) {
        
    }

    public async Task ResetWaterSensor(string device) {
        
    }

    public async Task SetMaintenance(string device,bool state) {
        
    }

    public async Task SetRemoteAlarm(string device,bool state) {
        
    }

    public async Task SetRemoteWarning(string device,bool state) {
        
    }
    
}