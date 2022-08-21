using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.SettingsModel;

public class EmailRecipient {
    public ObjectId _id { get; set; }
    public string Username { get; set; }
    public string Address { get; set; }
}