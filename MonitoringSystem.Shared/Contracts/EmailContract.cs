using MonitoringSystem.Shared.Data;
namespace MonitoringSystem.Shared.Contracts {
    public interface EmailContract {
        DateTime TimeStamp { get; set; }
        IList<AlertRecord> Alerts { get; set; }
    }
}
