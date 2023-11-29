using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.LogModel;
using MonitoringSystem.Shared.SignalR;

namespace MonitoringWeb.WebApp.Data; 

public class DeviceStatus {
    public string DeviceName { get; set; }
    public ActionType Status { get; set; }
    public IEnumerable<ItemStatus> ActiveAlerts { get; set; }
}

public class StatusItem {
    public string? Item { get; set; }
    public ActionType Status { get; set; }
    public string? Label { get; set; }
    public string? Value { get; set; }
    public string? TimeStamp { get; set; }
}

public class Nh3StatusItem {
    public string? Item { get; set; }
    public TankScaleState Status { get; set; }
    public string? Label { get; set; }
    public string? Value { get; set; }
    public string? TimeStamp { get; set; }
    public string? CssClass { get; set; }
}