using MongoDB.Bson;
namespace MonitoringSystem.Shared.Data;

public class SensorType {
    public int _id { get; set; }
    public string Name { get; set; }
    public string Units { get; set; }
    public int YAxisStart { get; set; }
    public int YAxisStop { get; set; }
}