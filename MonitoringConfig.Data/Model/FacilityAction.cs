namespace MonitoringConfig.Data.Model;


public class FacilityAction {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool EmailEnabled { get; set; }
    public int EmailPeriod { get; set; }
    public ActionType ActionType { get; set; }
    public ICollection<DeviceAction> DeviceActions { get; set; } = new List<DeviceAction>();
}

public class ActionOutput {
    public Guid Id { get; set; }
    public Guid DiscreteOutputId { get; set; }
    public DiscreteOutput? DiscreteOutput { get; set; }
    public Guid DeviceActionId { get; set; }
    public DeviceAction? DeviceAction { get; set; }
    
    public DiscreteState OnLevel { get; set; }
    public DiscreteState OffLevel{ get; set; }
}

public class DeviceAction {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int FirmwareId { get; set; }
    public Guid MonitorBoxId { get; set; }
    public MonitorBox? MonitorBox { get; set; }
    public Guid FacilityActionId { get; set; }
    public FacilityAction? FacilityAction { get; set; }
    public ICollection<ActionOutput> ActionOutputs { get; set; } = new List<ActionOutput>();
    public ICollection<AlertLevel> AlertLevels { get; set; } = new List<AlertLevel>();
}