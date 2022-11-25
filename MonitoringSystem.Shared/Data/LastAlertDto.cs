using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data; 

public class LastAlertDto {
    public string alertId { get; set; }
    public ObjectId channelId { get; set; }
    public string sensorId { get; set; }
    public AlertItemType ItemType { get; set; }
    public string Name { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Device { get; set; }
    public string database { get; set; }
    public string State { get; set; }
    public float Value { get; set; }
}