using System.Drawing;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data; 

public class WebsiteBulkSettings {
    public ObjectId _id { get; set; }
    public int RefreshTime { get; set; }
    public BulkGasSettings H2Settings { get; set; }
    public BulkGasSettings N2Settings { get; set; }
    public BulkGasSettings NHSettings { get; set; }
    public BulkGasSettings SiSettings { get; set; }
}

public class BulkEmailSettings {
    public ObjectId _id { get; set; }
    public string? Message { get; set; }
    public List<string> CcAddresses { get; set; } = new();
    public List<string> ToAddresses { get; set; } = new();
}

public class BulkGasSettings {
    public string? Name { get; set; }
    public string? DeviceName { get; set; }
    public string? ChannelName { get; set; }
    public BulkGasType BulkGasType { get; set; }
    public KnownColor PointColor { get; set; }
    public int HoursBefore { get; set; }
    public int HoursAfter { get; set; }
    public int YAxisMin { get; set; }
    public int YAxisMax { get; set; }
    public bool EnableAggregation { get; set; }
    public string? OkayLabel { get; set; }
    
    public BulkGasAlert AlrmAlert { get; set; }
    public BulkGasAlert WarnAlert { get; set; }
    public BulkGasAlert SoftAlert { get; set; }
    
    public RefLine AlrmRefLine { get; set; }
    public RefLine WarnRefLine { get; set; }
    public RefLine SoftRefLine { get; set; }
}

public enum NhTank:int {
    Tank1=0,
    Tank2
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