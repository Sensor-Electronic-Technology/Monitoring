using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data.LogModel {
    public class MonitorAlert {
        public ObjectId _id { get; set; }
        public string EntityId { get; set; }
        public string DisplayName { get; set; }
        public ObjectId ChannelId { get; set; }
        public AlertItemType AlertItemType { get; set; }
        public bool Enabled { get; set; }
        public bool Bypassed { get; set; }
        public int BypassResetTime { get; set; }
        public int Register { get; set; }
        public MonitorAlert Clone() {
            return (MonitorAlert)this.MemberwiseClone();
        }
    }
}
