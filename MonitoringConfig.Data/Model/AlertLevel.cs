namespace MonitoringConfig.Data.Model; 

public abstract class AlertLevel {
    public Guid Id { get; set; }
    public bool Bypass { get; set; }
    public int BypassResetTime { get; set; }
    public bool Enabled { get; set; }
    public AlertLevelAction? AlertLevelAction { get; set; }
}

public class AnalogLevel : AlertLevel {
    public double SetPoint { get; set; }
    public Guid AnalogAlertId { get; set; }
    public AnalogAlert? AnalogAlert { get; set; }
}

public class DiscrteLevel:AlertLevel {
    public DiscreteState TriggerOn { get; set; }
    public Guid DiscreteAlertId { get; set; }
    public DiscreteAlert? DiscreteAlert { get; set; }
}