namespace MonitoringConfig.Data.Model; 

public abstract class AlertLevel {
    public Guid Id { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public Guid AlertId { get; set; }
    public Alert? Alert { get; set; }
    public Guid DeviceActionId { get; set; }
    public DeviceAction? DeviceAction { get; set; }
    //public AlertLevelAction? AlertLevelAction { get; set; }
}

public class AnalogLevel : AlertLevel {
    public double SetPoint { get; set; }
}

public class DiscreteLevel:AlertLevel {
    public DiscreteState TriggerOn { get; set; }
}