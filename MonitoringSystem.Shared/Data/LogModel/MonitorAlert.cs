using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MonitoringSystem.Shared.Data {
    public class MonitorAlert {
        public ObjectId _id { get; set; }
        public string displayName { get; set; }
        public ObjectId channelId { get; set; }
        public AlertItemType itemType { get; set; }
        public bool enabled { get; set; }
        public bool bypassed { get; set; }
        public int bypassResetTime { get; set; }
        public bool latched { get; set; }
        public ActionType CurrentState { get; set; }
        public MonitorAlert Clone() {
            return (MonitorAlert)this.MemberwiseClone();
        }
    }
}
