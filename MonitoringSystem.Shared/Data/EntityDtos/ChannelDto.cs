namespace MonitoringSystem.Shared.Data.EntityDtos;

public abstract class ChannelDto {
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string? DisplayName { get; set; }
    public int SystemChannel { get; set; }
    public bool Connected { get; set; }
    public bool Bypass { get; set; }
    public bool Display { get; set; }
    public int RegisterAddress { get; set; }
    public int RegisterLength { get; set; }
    public ModbusRegister RegisterType { get; set; }
    public int ChannelAddress { get; set; }
    public int ModuleSlot { get; set; }
    public Guid ModbusDeviceId { get; set; }
}

public class AnalogInputDto:ChannelDto {
    public Guid? SensorId { get; set; }
}

public class DiscreteInputDto:ChannelDto { }

public class VirtualInputDto:ChannelDto { }

public class DiscreteOutputDto:ChannelDto {
    public DiscreteState StartState { get; set; }
}

public class SensorDto {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public double RecordThreshold { get; set; }
    public double Slope { get; set; }
    public double Offset { get; set; }
    public double Factor { get; set; }
    public string? Units { get; set; }
    public int YAxisMin { get; set; }
    public int YAxisMax { get; set; }
}


