using System.Drawing;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data; 

public class WebsiteBulkSettings {
    public ObjectId _id { get; set; }
    public int RefreshTime { get; set; }
    public BulkGasSettings? H2Settings { get; set; }
    public BulkGasSettings? N2Settings { get; set; }
}

public class BulkGasSettings {
    public KnownColor PointColor { get; set; }
    public int HoursBefore { get; set; }
    public int HoursAfter { get; set; }
    public bool EnableAggregation { get; set; }
    
    public List<RefLine> ReferenceLines { get; set; } = new();
    
    public string? OkayLabel { get; set; }
    public BulkGasAlert? SoftWarnAlert { get; set; }
    public BulkGasAlert? WarningAlert { get; set; }
    public BulkGasAlert? AlarmAlert { get; set; }
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