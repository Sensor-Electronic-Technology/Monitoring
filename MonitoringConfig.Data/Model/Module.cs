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
    
    //public ICollection<MonitorBox> MoitorBoxes { get; set; }
    public ICollection<BoxModule> BoxModules { get; set; } = new List<BoxModule>();


}

public class BoxModule {
    public Guid Id { get; set; }
    
    public Guid ModuleId { get; set; }
    public Module Module { get; set; }
    
    public Guid MonitorBoxId { get; set; }
    public MonitorBox MonitorBox { get; set; }
    
    public int ModuleSlot { get; set; }
    public List<Channel> Channels { get; set; } = new List<Channel>();
}