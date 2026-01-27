using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.LogModel {
    public class MonitorAlert {
        public ObjectId _id { get; set; }
        public string EntityId { get; set; }
        public string DisplayName { get; set; }
        public ObjectId ChannelId { get; set; }
        public AlertItemType AlertItemType { get; set; }
        public bool Display { get; set; }
        public bool Enabled { get; set; }
        public bool Bypassed { get; set; }
        public int BypassResetTime { get; set; }
        public int Register { get; set; }
        public DateTime TimeBypassed { get; set; } = DateTime.MaxValue;
        public MonitorAlert Clone() {
            return (MonitorAlert)this.MemberwiseClone();
        }
    }
}
