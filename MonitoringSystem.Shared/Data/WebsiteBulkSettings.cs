using System.Drawing;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data; 

public class WebsiteBulkSettings {
    public ObjectId _id { get; set; }
    public int RefreshTime { get; set; }

    public List<BulkGasSettings> BulkGasSettings { get; set; } = new();
}

public class BulkGasSettings {
    public string? Name { get; set; }
    public BulkGasType BulkGasType { get; set; }
    public KnownColor PointColor { get; set; }
    public int HoursBefore { get; set; }
    public int HoursAfter { get; set; }
    public bool EnableAggregation { get; set; }
    public string? OkayLabel { get; set; }
    public List<BulkGasAlert> BulkGasAlerts { get; set; } = new();
    public List<RefLine> ReferenceLines { get; set; } = new();
}

public class RefLine {
    public string? Label { get; set; }
    public int Value { get; set; }
    public KnownColor Color { get; set; }
}

public class BulkGasAlert {
    public ActionType ActionType { get; set; }
    public string? Label { get; set; }
    public int SetPoint { get; set; }
    public bool Default { get; set; }
}