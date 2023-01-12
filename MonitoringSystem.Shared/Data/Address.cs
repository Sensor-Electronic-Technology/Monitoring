namespace MonitoringSystem.Shared.Data; 

public class ChannelAddress {
    public int Channel { get; set; }
    public int ModuleSlot { get; set; }
}

public class ModbusAddress {
    public int Address { get; set; }
    public int RegisterLength { get; set; }
    public ModbusRegister RegisterType { get; set; }
}