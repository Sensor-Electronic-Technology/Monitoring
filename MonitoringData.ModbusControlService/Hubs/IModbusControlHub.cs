namespace MonitoringData.ModbusControlService.Services; 

public interface IModbusControlHub {
    Task ToggleMaintenance();
    Task ToggleRemoteAlarm();
    Task ToggleRemoteWarning();
    Task ResetWaterSensor();

    Task SetMaintenance(bool state);
    Task SetRemoteAlarm(bool state);
    Task SetRemoteWarning(bool state);
}