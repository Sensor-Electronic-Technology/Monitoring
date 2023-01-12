namespace MonitoringConfig.Data.Model;

public enum ModuleType {
    AnalogInput,
    AnalogOutput,
    DiscreteInput,
    DiscreteOutput
}

public class Module {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public ModuleType ModuleType { get; set; }
    public int ChannelCount { get; set; }
    
    public ICollection<MonitorBox> MoitorBoxes { get; set; }
    public List<MonitorBoxModule> MonitorBoxModules { get; set; }

    //public ICollection<Channel> Channels { get; set; } = new List<Channel>();
}

public class MonitorBoxModule {
    public Guid ModuleId { get; set; }
    public Module Module { get; set; }
    
    public Guid MonitorBoxId { get; set; }
    public MonitorBox ModuleBox { get; set; }
    
    public int ModuleSlot { get; set; }
}