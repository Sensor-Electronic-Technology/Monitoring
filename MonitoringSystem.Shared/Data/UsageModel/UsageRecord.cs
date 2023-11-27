using System.Globalization;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.UsageModel;

public class UsageDayRecord {
    public ObjectId _id { get; set; }
    public List<ObjectId> ChannelIds { get; set; } = new List<ObjectId>();
    public DateTime Date { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string? MonthName { get; set; }
    public int WeekOfYear { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public int DayOfYear { get; set; }
    public int DayOfMonth { get; set; }
    public double Usage { get; set; }
    public double PerSec { get; set; }
    public double PerMin { get; set; }
    public double PerHour { get; set; }
}