using MongoDB.Bson;
using MonitoringSystem.Shared.Data.UsageModel;

namespace MonitoringWeb.WebApp.Data; 

public class UsageRecordDto {
    public ObjectId _id { get; set; }
    public ObjectId ChannelId { get; set; }
    public string Identifier { get; set; }
    public int Year { get; set; }
    public List<MonthRecord> MonthRecords { get; set; } = new List<MonthRecord>();
    public double PerSec { get; set; }
    public double PerMin { get; set; }
    public double PerHour { get; set; }
    public double PerDay { get; set; }
    public double PerWeek { get; set; }
    public double PerMonth { get; set; }
    public double PerYear { get; set; }
}