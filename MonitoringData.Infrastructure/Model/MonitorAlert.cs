using MonitoringSystem.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Model {
    public class MonitorAlert {
        public int _id { get; set; }
        public string displayName { get; set; }
        public int channelId { get; set; }
        public AlertItemType itemType { get; set; }
        public bool enabled { get; set; }
        public bool bypassed { get; set; }
        public int bypassResetTime { get; set; }
        public DateTime bypassedTime { get; set; }
        public DateTime lastAlarm { get; set; }
        public bool latched { get; set; }
        public ActionType CurrentState { get; set; }

        public MonitorAlert Clone() {
            return (MonitorAlert)this.MemberwiseClone();
        }
    }
}
