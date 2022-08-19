namespace MonitoringConfig.Data.Model;

public abstract class Channel {
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string? DisplayName { get; set; }
    
    public int SystemChannel { get; set; }
    public bool Connected { get; set; }
    public bool Bypass { get; set; }
    public bool Display { get; set; }
    
    public ModbusAddress? ModbusAddress { get; set; }
    public ChannelAddress? ChannelAddress { get; set; }
    
    public Guid ModbusDeviceId { get; set; }
    public ModbusDevice? ModbusDevice { get; set; }
}

public class InputChannel:Channel {
    public Alert? Alert { get; set; }
}

public class OutputChannel : Channel {
    public ICollection<ActionOutput> ActionOutputs { get; set; } = new List<ActionOutput>();
}

public class AnalogInput:InputChannel{
    public Guid? SensorId { get; set; }
    public Sensor? Sensor { get; set; }
}

public class DiscreteInput : InputChannel { }

public class VirtualInput : InputChannel { }

public class DiscreteOutput : OutputChannel {
    public DiscreteState StartState { get; set; }
}

