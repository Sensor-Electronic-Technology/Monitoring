using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringWeb.WebApp.Data; 

public class MonitorWebsiteSettings : MonitorSettings {
    public string? ManagedDeviceCollection { get; set; } = null!;
    public string? SensorTypeCollection { get; set; } = null!;
    public string? BulkSettingsCollection { get; set; } = null!;
    public string? BulkEmailSettingsCollection { get; set; } = null!;
}