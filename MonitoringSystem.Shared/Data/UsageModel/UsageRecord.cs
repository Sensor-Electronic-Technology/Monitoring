using System.Globalization;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.UsageModel;

public class DayRecord {
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

public class MonthRecord {
    public ObjectId _id { get; set; }
    public List<ObjectId> ChannelIds { get; set; } = new List<ObjectId>();
    public string? Identifier { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string? MonthName { get; set; }
    public DateTime TimeStamp { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }
    public List<DayRecord> DayRecords { get; set; }= new List<DayRecord>();
    public double Usage { get; set; }
    public double PerSec { get; set; }
    public double PerMin { get; set; }
    public double PerHour { get; set; }
    public double PerDay { get; set; }

}

/*public class UsageRecord {
    public ObjectId _id { get; set; }
    public ObjectId ChannelId { get; set; }
    public string Identifier { get; set; }
    public int Year { get; set; }
    public List<ObjectId> MonthRecords { get; set; } = new List<ObjectId>();
    
    public double PerSec { get; set; }
    public double PerMin { get; set; }
    public double PerHour { get; set; }
    public double PerDay { get; set; }
    public double PerWeek { get; set; }
    public double PerMonth { get; set; }
    public double PerYear { get; set; }
}*/