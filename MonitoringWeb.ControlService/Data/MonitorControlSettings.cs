using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringWeb.ControlService.Data;

public class MonitorControlSettings:MonitorSettings {
    public string ManagedDeviceCollection { get; set; }= null!;
    public string VirtualChannelCollection { get; set; }= null!;
}