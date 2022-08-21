using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringWeb.WebAppV2.Data; 

public class MonitorWebsiteSettings : MonitorSettings {
    public string? ManagedDeviceCollection { get; set; } = null!;
    public string? SensorTypeCollection { get; set; } = null!;
}