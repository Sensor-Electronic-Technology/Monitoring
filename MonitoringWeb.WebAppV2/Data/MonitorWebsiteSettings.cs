using MonitoringSystem.Shared.Data;
namespace MonitoringWeb.WebAppV2.Data; 

public class MonitorWebsiteSettings : MonitorSettings {
    public string? ManagedDeviceCollection { get; set; } = null!;
}