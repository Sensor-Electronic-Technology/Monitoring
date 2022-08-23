namespace MonitoringSystem.Shared.Data.EntityDtos; 

public class ModbusDeviceDto {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int ReadInterval { get; set; }
    public int SaveInterval { get; set; }
    public string? Database { get; set; }
    public string? HubName { get; set; }
    public string? HubAddress { get; set; }
    public NetworkConfigDto NetworkConfig { get; set; } = default!;
    public ModbusConfigDto ModbusConfig { get; set; }= default!;
    public ChannelMappingConfigDto RegisterMapping { get; set; }= default!;
}