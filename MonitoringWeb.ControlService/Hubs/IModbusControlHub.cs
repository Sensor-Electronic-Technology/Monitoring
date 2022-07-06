namespace MonitoringWeb.ControlService.Hubs; 


public interface IModbusControlHub {
    Task ToggleMaintenance(string device);
    Task ToggleRemoteAlarm(string device);
    Task ToggleRemoteWarning(string device);
    Task ResetWaterSensor(string device);

    Task SetMaintenance(string device,bool state);
    Task SetRemoteAlarm(string device,bool state);
    Task SetRemoteWarning(string device,bool state);
}