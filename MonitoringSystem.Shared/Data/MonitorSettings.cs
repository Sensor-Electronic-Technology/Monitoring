using MongoDB.Bson;
namespace MonitoringSystem.Shared.Data;

public abstract class MonitorSettings {
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}

public class ManagedDevice {
    public ObjectId _id { get; set; }
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
    public ChannelRegisterMapping ChannelMapping { get; set; }
    public ModbusConfig ModbusConfiguration { get; set; }
}

public class EmailRecipient {
    public ObjectId _id { get; set; }
    public string Username { get; set; }
    public string Address { get; set; }
}
public class RemoteAction {
    public string Name { get; set; }
    public bool State { get; set; }
    public int Register { get; set; }
}