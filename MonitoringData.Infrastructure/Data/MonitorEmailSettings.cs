using MonitoringSystem.Shared.Data;

namespace MonitoringData.Infrastructure.Data;
public class MonitorEmailSettings {
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string FromUser { get; set; }
    public string FromAddress { get; set; }
    public string ExternalFromUser { get; set; }
    public string ExternalFromAddress { get; set; }
}