namespace MonitoringSystem.Shared.Data.EntityDtos;


public abstract class ChannelDto {
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string? DisplayName { get; set; }
    public int SystemChannel { get; set; }
    public bool Connected { get; set; }
    public bool Bypass { get; set; }
    public bool Display { get; set; }
    public ModbusAddress? ModbusAddress { get; set; }
    public ChannelAddress? ChannelAddress { get; set; }
}

public class AnalogInputDto:ChannelDto {
    public AnalogAlertDto? Alert { get; set; }
    public SensorDto? Sensor { get; set; }
}

public class DiscreteInputDto:ChannelDto {
    public DiscreteAlertDto? Alert { get; set; }
}

public class VirtualInputDto:ChannelDto {
    public DiscreteAlertDto? Alert { get; set; }
}

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


