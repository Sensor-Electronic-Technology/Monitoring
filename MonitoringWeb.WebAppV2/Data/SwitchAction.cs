namespace MonitoringWeb.WebAppV2.Data; 

public class SwitchAction {
    public string? Name { get; set; }
    public bool State { get; set; }
    public string? SwitchId { get; set; }
    public Func<string,Task> Toggle;
}