namespace MonitoringConfig.Data.Model; 

public abstract class Device {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int ReadInterval { get; set; }
    public int SaveInterval { get; set; }
    public string? Database { get; set; }
    public string? HubName { get; set; }
    public string? HubAddress { get; set; }
}

public class ModbusDevice:Device {
    public NetworkConfiguration? NetworkConfiguration { get; set; }
    public ModbusConfiguration? ModbusConfiguration { get; set; }
    public ModbusChannelRegisterMap? ChannelRegisterMap { get; set; }
    public ICollection<DeviceAction> DeviceActions { get; set; } = new List<DeviceAction>();
    public ICollection<Channel> Channels { get; set; } = new List<Channel>();
}

public class MonitorBox : ModbusDevice {
    //public ICollection<Module> Modules { get; set; }
    public ICollection<BoxModule> BoxModules { get; set; } = new List<BoxModule>();
}

