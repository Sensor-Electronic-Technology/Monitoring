namespace MonitoringConfig.Data.Model;

public enum ModbusRegister {
    Holding = 1,
    Coil = 2,
    DiscreteInput = 3,
    Input = 4
}

public class ChannelAddress {
    public int Channel { get; set; }
    public int ModuleSlot { get; set; }
}

public class ModbusAddress {
    public int Address { get; set; }
    public int RegisterLength { get; set; }
    public ModbusRegister RegisterType { get; set; }
}