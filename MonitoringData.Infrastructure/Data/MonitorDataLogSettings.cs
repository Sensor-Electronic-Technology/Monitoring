using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Data; 

public class MonitorDataLogSettings:MonitorSettings {
    public string ManagedDeviceCollection { get; set; } = null!;
    public string EmailRecipientCollection { get; set; } = null!;
    public ServiceType ServiceType { get; set; } = ServiceType.MonitorBox;
}