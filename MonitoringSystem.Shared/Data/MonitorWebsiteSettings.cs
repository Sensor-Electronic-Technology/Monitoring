using MonitoringSystem.Shared.Data.SettingsModel;
namespace MonitoringSystem.Shared.Data; 

public class MonitorWebsiteSettings : MonitorSettings {
    public string? ManagedDeviceCollection { get; set; } = null!;
    public string? SensorTypeCollection { get; set; } = null!;
    public string? BulkSettingsCollection { get; set; } = null!;
    public string? BulkEmailSettingsCollection { get; set; } = null!;
    public string? BulkH2CalcSettingsCollection { get; set; } = null!;
    public string? NH3LogDatabase { get; set; } = null!;
    public string? TankScaleCollection { get; set; } = null!;
}