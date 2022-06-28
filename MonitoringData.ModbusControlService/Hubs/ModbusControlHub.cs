using Microsoft.AspNetCore.SignalR;
using MonitoringSystem.Shared.Services;

namespace MonitoringData.ModbusControlService.Services;

public class MonitorControlHub : Hub<IModbusControlHub> {
    private readonly IModbusService _modbusService;
    private readonly IMonitorDeviceService _deviceService;
    
    public MonitorControlHub(IModbusService modbusService,IMonitorDeviceService deviceService) {
        this._modbusService = modbusService;
        this._deviceService = deviceService;
    }

    public async Task ToggleMaintenance() {
        
    }

    public async Task ToggleRemoteAlarm() {
        
    }

    public async Task ToggleRemoteWarning() {
        
    }

    public async Task ResetWaterSensor() {
        
    }

    public async Task SetMaintenance(bool state) {
        
    }

    public async Task SetRemoteAlarm(bool state) {
        
    }

    public async Task SetRemoteWarning(bool state) {
        
    }
    
}