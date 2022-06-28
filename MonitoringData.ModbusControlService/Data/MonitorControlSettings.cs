namespace MonitoringData.ModbusControlService.Data; 

public class MonitorControlSettings {
    public string ConnectionString { get; set; } = null!;
    public string DeviceControllerCollection { get; set; }
    public string VirtualChannelCollection { get; set; }
}