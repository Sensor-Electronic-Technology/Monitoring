using System.Globalization;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.UsageModel;

public class DayRecord {
    public int Day { get; set; }
    public DateTime TimeStamp { get; set; }
    public DateTime StartDate { get; set; }
    public string Name { get; set; }
    public double Consumption { get; set; }
    public double PerSec { get; set; }
    public double PerMin { get; set; }
    public double PerHour { get; set; }
}

public class MonthRecord {
    public int Month { get; set; }
    public string Name { get; set; }
    public DateTime TimeStamp { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }
    public double Consumption { get; set; }
    public double PerSec { get; set; }
    public double PerMin { get; set; }
    public double PerHour { get; set; }
    public double PerDay { get; set; }
    public List<DayRecord> DayRecords { get; set; }= new List<DayRecord>();
}

public class UsageRecord {
    public ObjectId _id { get; set; }
    public ObjectId ChannelId { get; set; }
    public string Identifier { get; set; }
    public DateTime TimeStamp { get; set; }
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