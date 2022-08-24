using MongoDB.Bson;
using MonitoringSystem.Shared.Data.EntityDtos;
using MonitoringSystem.Shared.Data.LogModel;

namespace MonitoringSystem.Shared.Data.SettingsModel;

public class ManagedDevice {
    public ObjectId _id { get; set; }
    public string DeviceId { get; set; }
    public string? DatabaseName { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? HubAddress { get; set; }
    public string? HubName { get; set; }
    public int RecordInterval { get; set; }
    public Dictionary<string, string> CollectionNames { get; set; } = new Dictionary<string, string>();
    public List<ObjectId> SensorTypes { get; set; } = new List<ObjectId>();
    public List<RemoteAction> RemoteActions { get; set; } = new List<RemoteAction>();
    public string? IpAddress { get; set; }
    public int Port { get; set; }
    public ChannelMappingConfigDto? ChannelMapping { get; set; }
    public ModbusConfigDto? ModbusConfiguration { get; set; }
}