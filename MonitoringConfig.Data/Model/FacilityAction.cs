using MonitoringSystem.Shared.Data;

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
    public Guid? DiscreteOutputId { get; set; }
    public DiscreteOutput? DiscreteOutput { get; set; }
    public Guid? DeviceActionId { get; set; }
    public DeviceAction? DeviceAction { get; set; }
    public DiscreteState OnLevel { get; set; }
    public DiscreteState OffLevel{ get; set; }
}