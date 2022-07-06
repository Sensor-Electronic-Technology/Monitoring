namespace MonitoringWeb.ControlService.Data;

public class MonitorControlSettings {
    public string ConnectionString { get; set; } = null!;
    public string ManagedDeviceCollection { get; set; }= null!;
    public string VirtualChannelCollection { get; set; }= null!;
}