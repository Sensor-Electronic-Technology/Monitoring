using MonitoringSystem.Shared.Data;

namespace MonitoringConfig.Data.Model; 

public abstract class AlertLevel {
    public Guid Id { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public Guid? DeviceActionId { get; set; }
    public DeviceAction? DeviceAction { get; set; }
    //public AlertLevelAction? AlertLevelAction { get; set; }
}

public class AnalogLevel : AlertLevel {
    public Guid AnalogAlertId { get; set; }
    public AnalogAlert? AnalogAlert { get; set; }
    public double SetPoint { get; set; }
}

public class DiscreteLevel:AlertLevel {
    public Guid DiscreteAlertId { get; set; }
    public DiscreteAlert? DiscreteAlert { get; set; }
    public DiscreteState TriggerOn { get; set; }
}