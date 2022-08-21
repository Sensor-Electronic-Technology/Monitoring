using Microsoft.AspNetCore.SignalR;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.SettingsModel;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.ControlService.Services;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringWeb.ControlService.Hubs;

public class MonitorControlHub : Hub<IModbusControlHub> {
    private readonly IModbusService _modbusService;
    private readonly IMonitorDeviceService _deviceService;
    private IEnumerable<ManagedDevice> _devices;
    public MonitorControlHub(IModbusService modbusService,IMonitorDeviceService deviceService) {
        this._modbusService = modbusService;
        this._deviceService = deviceService;
    }
    
    public async Task StartAsync() {
        await this._deviceService.Load();
        this._devices = this._deviceService.AvailableDevices;
    }
    
    public async Task Initialize(string deviceName) {
        var device = this._devices.FirstOrDefault(e => e.DeviceName == deviceName);
        if (device == null) {
            await Clients.Caller.InitializeActions(Enumerable.Empty<RemoteAction>());
            return;
        }
        foreach (var remoteAction in device.RemoteActions) {
            var state=await this._modbusService.ReadCoil(device.IpAddress, device.Port, 1, remoteAction.Register);
            remoteAction.State = state;
        }
        await Clients.Caller.InitializeActions(device.RemoteActions.AsEnumerable());
    }
    
    public async Task Toggle(string deviceName,string actionName) {
        var device = this._devices.FirstOrDefault(e => e.DeviceName == deviceName);
        if (device == null) {
            return;
        }
        var remoteAction = device.RemoteActions.FirstOrDefault(e => e.Name == actionName);
        if (remoteAction == null) {
            return;
        }
        await this._modbusService.ToggleCoil(device.IpAddress, device.Port,1,remoteAction.Register);
    }
    
    

    /*/*public async Task ToggleMaintenance(string deviceName) {
        var device = this._devices.FirstOrDefault(e => e.Device.DeviceName == deviceName);
        if (device != null) {
            var ip = device.MonitorDevice.NetworkConfiguration.IPAddress;
            var slaveId = device.MonitorDevice.NetworkConfiguration.ModbusConfig.SlaveAddress;
            var port = device.MonitorDevice.NetworkConfiguration.Port;
            var channel = device.VirtualChannels.FirstOrDefault(e => e.identifier == "Remote Maint.");
            if (channel != null) {
                await this._modbusService.ToggleCoil(ip, port, slaveId, channel.register);
            }
        }
    }#1#

    public async Task ToggleRemoteAlarm(string deviceName) {
        var device = this._devices.FirstOrDefault(e => e.Device.DeviceName == deviceName);
        if (device != null) {
            var ip = device.MonitorDevice.NetworkConfiguration.IPAddress;
            var slaveId = device.MonitorDevice.NetworkConfiguration.ModbusConfig.SlaveAddress;
            var port = device.MonitorDevice.NetworkConfiguration.Port;
            var channel = device.VirtualChannels.FirstOrDefault(e => e.identifier == "Remote Alarm");
            if (channel != null) {
                await this._modbusService.ToggleCoil(ip, port, slaveId, channel.register);
            }
        }
    }

    public async Task ToggleRemoteWarning(string deviceName) {
        var device = this._devices.FirstOrDefault(e => e.Device.DeviceName == deviceName);
        if (device != null) {
            var ip = device.MonitorDevice.NetworkConfiguration.IPAddress;
            var slaveId = device.MonitorDevice.NetworkConfiguration.ModbusConfig.SlaveAddress;
            var port = device.MonitorDevice.NetworkConfiguration.Port;
            var channel = device.VirtualChannels.FirstOrDefault(e => e.identifier == "Remote Warning");
            if (channel != null) {
                await this._modbusService.ToggleCoil(ip, port, slaveId, channel.register);
            }
        }
    }

    public async Task ResetWaterSensor(string deviceName) {
        var device = this._devices.FirstOrDefault(e => e.Device.DeviceName == deviceName);
        if (device != null) {
            var ip = device.MonitorDevice.NetworkConfiguration.IPAddress;
            var slaveId = device.MonitorDevice.NetworkConfiguration.ModbusConfig.SlaveAddress;
            var port = device.MonitorDevice.NetworkConfiguration.Port;
            var channel = device.VirtualChannels.FirstOrDefault(e => e.identifier == "Remote Water Reset");
            if (channel != null) {
                await this._modbusService.ToggleCoil(ip, port, slaveId, channel.register);
            }
        }
    }*/
}