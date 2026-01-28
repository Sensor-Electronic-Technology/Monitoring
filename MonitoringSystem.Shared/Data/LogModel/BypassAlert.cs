using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.LogModel;

public class BypassAlert {
    public ObjectId _id { get; set; }
    public bool Enabled { get; set; }
    public string DeviceName { get; set; } = "Not Set";
    public bool Bypassed { get; set; }
    public int BypassResetTime { get; set; }
    public DateTime TimeBypassed { get; set; }=DateTime.MaxValue;
    
    public void CopyFrom(BypassAlert other) {
        this.Enabled = other.Enabled;
        this.DeviceName = other.DeviceName;
        this.Bypassed = other.Bypassed;
        this.BypassResetTime = other.BypassResetTime;
        this.TimeBypassed = other.TimeBypassed;
    }

}