using Microsoft.AspNetCore.SignalR;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Services;
using MonitoringWeb.ControlService.Services;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringWeb.ControlService.Hubs;

public class MonitorControlHub : Hub<IModbusControlHub> {
    private readonly IModbusService _modbusService;
    private readonly IMonitorDeviceService _deviceService;
    private IEnumerable<DeviceControlData> _devices;
    public MonitorControlHub(IModbusService modbusService,IMonitorDeviceService deviceService) {
        this._modbusService = modbusService;
        this._deviceService = deviceService;
    }
    public async Task StartAsync() {
        await this._deviceService.Load();
        this._devices = this._deviceService.AvailableDevices;
    }
    public async Task<IEnumerable<RemoteAction>?> Initialize(string deviceName) {
        var device = this._devices.FirstOrDefault(e => e.Device.DeviceName == deviceName);
        if (device == null) {
            return null;
        }
        var ip = device.MonitorDevice.NetworkConfiguration.IPAddress;
        var slaveId = device.MonitorDevice.NetworkConfiguration.ModbusConfig.SlaveAddress;
        var registers = device.MonitorDevice.NetworkConfiguration.ModbusConfig.Coils;
        var port = device.MonitorDevice.NetworkConfiguration.Port;

        var coils = await this._modbusService.ReadCoils(ip, port, slaveId, 0, registers);
        if (coils == null) {
            return null;
        }
        List<RemoteAction> remoteActions = new List<RemoteAction>();
        if (device.VirtualChannels.Count() == coils.Length) {
            for (int i = 0; i < coils.Length; i++) {
                remoteActions.Add(new RemoteAction() {
                    Name=device.VirtualChannels[i].identifier,
                    State = coils[i]
                });
            }
        }
        return remoteActions.AsEnumerable();
    }

    public async Task Toggle(string deviceName,string actionName) {
        var device = this._devices.FirstOrDefault(e => e.Device.DeviceName == deviceName);
        if (device == null) {
            return;
        }
        var ip = device.MonitorDevice.NetworkConfiguration.IPAddress;
        var slaveId = device.MonitorDevice.NetworkConfiguration.ModbusConfig.SlaveAddress;
        var port = device.MonitorDevice.NetworkConfiguration.Port;
        var channel = device.VirtualChannels.FirstOrDefault(e => e.identifier == actionName);
        if (channel == null) {
            return;
        }
        await this._modbusService.ToggleCoil(ip, port, slaveId, channel.register);
    }

    public async Task ToggleMaintenance(string deviceName) {
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
    }

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
    }

    public async Task SetMaintenance(string deviceName,bool state) {
        
    }

    public async Task SetRemoteAlarm(string deviceName,bool state) {
        
    }

    public async Task SetRemoteWarning(string deviceName,bool state) {
        
    }
}