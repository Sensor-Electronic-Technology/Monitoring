using MonitoringSystem.Shared.Data;

namespace MonitoringWeb.ControlService.Data;

public class MonitorControlSettings:MonitorSettings {
    public string ManagedDeviceCollection { get; set; }= null!;
    public string VirtualChannelCollection { get; set; }= null!;
}