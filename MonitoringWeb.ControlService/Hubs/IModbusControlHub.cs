namespace MonitoringWeb.ControlService.Hubs; 


public interface IModbusControlHub {
    Task Toggle(string device);
    Task SetMaintenance(string device,bool state);
    Task SetRemoteAlarm(string device,bool state);
    Task SetRemoteWarning(string device,bool state);
}