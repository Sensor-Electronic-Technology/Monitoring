using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.SettingsModel;

public class BulkH2CalcSettings {
    public ObjectId _id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int DaysFromAlarm { get; set; }
    public double MinSoftWarnLevel { get; set; }
    public bool IncludeWeekends { get; set; }
}