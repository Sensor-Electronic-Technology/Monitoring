using MongoDB.Bson;
namespace MonitoringSystem.Shared.Data;

public abstract class MonitorSettings {
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}

public class MonitorEmailSettings:MonitorSettings {
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string FromUser { get; set; }
    public string FromAddress { get; set; }
    public string EmailRecipientCollection { get; set; }
}

public class ManagedDevice {
    public ObjectId _id { get; set; }
    public string DatabaseName { get; set; }
    public string DeviceName { get; set; }
    public string DeviceType { get; set; }
    public string HubAddress { get; set; }
    public int RecordInterval { get; set; }
    public Dictionary<string,string> CollectionNames { get; set; }
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