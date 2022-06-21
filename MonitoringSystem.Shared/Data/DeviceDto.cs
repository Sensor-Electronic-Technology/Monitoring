namespace MonitoringSystem.Shared.Data;

public class DeviceDto {
    public string Identifier { get; set; }
    public string DatabaseName { get; set; }
    public NetworkConfiguration NetworkConfiguration { get; set; }
}