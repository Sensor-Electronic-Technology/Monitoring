using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.SettingsModel;

public class BulkH2CalcSettings {
    public ObjectId _id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int DaysFromAlarm { get; set; }
    public double MinSoftWarnLevel { get; set; }
    public double AlarmLevel { get; set; } = 300;
    public double Rate { get; set; } = 1.0;
    public bool IncludeWeekends { get; set; }

    public static BulkH2CalcSettings Init() {
        return new BulkH2CalcSettings() {
            TimeStamp = DateTime.Now,
            DaysFromAlarm = 3,
            MinSoftWarnLevel = 400,
            AlarmLevel = 300,
            Rate = 1.0,
            IncludeWeekends = true,
            _id = ObjectId.GenerateNewId()
        };
    }
}