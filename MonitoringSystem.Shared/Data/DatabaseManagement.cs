using MongoDB.Bson;
namespace MonitoringSystem.Shared.Data;
public class ManagedDevice {
    public ObjectId _id { get; set; }
    public string DatabaseName { get; set; }
    public string DeviceName { get; set; }
    public string DeviceType { get; set; }
    public string HubAddress { get; set; }
}