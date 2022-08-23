using MongoDB.Bson;
using MonitoringSystem.Shared.Data.EntityDtos;

namespace MonitoringSystem.Shared.Data.SettingsModel;

public class ManagedDevice {
    public ObjectId _id { get; set; }
    public string DeviceId { get; set; }
    public string DatabaseName { get; set; }
    public string DeviceName { get; set; }
    public string DeviceType { get; set; }
    public string HubAddress { get; set; }
    public string HubName { get; set; }
    public int RecordInterval { get; set; }
    public Dictionary<string,string> CollectionNames { get; set; }
    public ObjectId[] SensorTypes { get; set; }
    public RemoteAction[] RemoteActions { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public ChannelMappingConfigDto ChannelMapping { get; set; }
    public ModbusConfigDto ModbusConfiguration { get; set; }
}