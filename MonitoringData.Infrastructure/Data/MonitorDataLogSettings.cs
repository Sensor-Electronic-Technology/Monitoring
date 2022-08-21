using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.SettingsModel;

namespace MonitoringData.Infrastructure.Data; 

public class MonitorDataLogSettings:MonitorSettings {
    public string ManagedDeviceCollection { get; set; } = null!;
    public string EmailRecipientCollection { get; set; } = null!;
}