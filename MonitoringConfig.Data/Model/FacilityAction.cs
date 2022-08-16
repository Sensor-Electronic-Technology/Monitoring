namespace MonitoringConfig.Data.Model; 

public class ActionOutput {
    public Guid Id { get; set; }
    
    public Guid DiscreteOutputId { get; set; }
    public DiscreteOutput? DiscreteOutput { get; set; }
    
    public Guid AlertLevelActionId { get; set; }
    public AlertLevelAction? AlertLevelAction { get; set; }
    
    public DiscreteState OnLevel { get; set; }
    public DiscreteState OffLevel{ get; set; }
}

public class FacilityAction {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int FirmwareId { get; set; }
    public bool EmailEnabled { get; set; }
    public int EmailPeriod { get; set; }
    public ActionType ActionType { get; set; }
    public ICollection<AlertLevelAction> AlertLevelActions { get; set; } = new List<AlertLevelAction>();
}

public class AlertLevelAction {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    
    public Guid AlertLevelId { get; set; }
    public AlertLevel? AlertLevel { get; set; }
    
    public Guid FacilityActionId { get; set; }
    public FacilityAction? FacilityAction { get; set; }

    public ICollection<ActionOutput> ActionOutputs { get; set; } = new List<ActionOutput>();
}