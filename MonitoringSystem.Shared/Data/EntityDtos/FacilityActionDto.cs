namespace MonitoringSystem.Shared.Data.EntityDtos; 

public class FacilityActionDto {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool EmailEnabled { get; set; }
    public int EmailPeriod { get; set; }
    public ActionType ActionType { get; set; }
}

public class ActionOutputDto {
    public Guid Id { get; set; }
    public Guid? DiscreteOutputId { get; set; }
    public Guid? DeviceActionId { get; set; }
    public DiscreteState OnLevel { get; set; }
    public DiscreteState OffLevel { get; set; }
}

public class DeviceActionDto {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int FirmwareId { get; set; }
    public ActionType ActionType { get; set; }
    public Guid FacilityActionId { get; set; }
    public Guid MonitorBoxId { get; set; }
}