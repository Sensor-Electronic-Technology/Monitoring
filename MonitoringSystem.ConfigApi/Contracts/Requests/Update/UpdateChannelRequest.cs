using MonitoringConfig.Data.Model;
using MonitoringSystem.Shared.Data;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.ConfigApi.Contracts.Requests.Update; 

public abstract class UpdateChannelRequest {
    public Guid ChannelId { get; set; }
    public string? Identifier { get; set; }
    public string? DisplayName { get; set; }
    
    public int SystemChannel { get; set; }
    public bool Connected { get; set; }
    public bool Bypass { get; set; }
    public bool Display { get; set; }
    public ModbusAddress? ModbusAddress { get; set; }
    public ChannelAddress? ChannelAddress { get; set; }
}

public class UpdateAnalogChannelRequest {
    public Guid SensorId { get; set; }
}

public class UpdateDiscreteChannelRequest { }

public class UpdateVirtualChannelRequest { }