namespace MonitoringWeb.WebApp.Data; 

public class SwitchAction {
    public string? Name { get; set; }
    public string? DeviceName { get; set; }
    public bool State { get; set; }
    public string? SwitchId { get; set; }
    public Func<string,string,Task> Toggle;
}